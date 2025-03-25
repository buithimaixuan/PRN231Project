using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Controllers
{
    [Route("Services")] // Định nghĩa base route cho controller
    public class ServicesController : Controller
    {
        private readonly HttpClient client = null;
        private string ServiceApiUrl = "";
        private string AccountApiUrl = "";
        private string BookingApiUrl = "";
        private string RatingApiUrl = "";
        private string CateServiceUrl = "";
        private const int PageSize = 8;

        public ServicesController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ServiceApiUrl = "https://localhost:7243/api/Services";
            AccountApiUrl = "https://localhost:7272/api/Accounts";
            BookingApiUrl = "https://localhost:7243/api/Booking";
            RatingApiUrl = "https://localhost:7243/api/Rating";
            CateServiceUrl = "https://localhost:7243/api/CateServices";
        }

        // Khai báo list chứa các dịch vụ
        public IEnumerable<Client.Models.Service> ServiceList { get; set; }
        // Dictionary để lưu tài khoản của từng dịch vụ
        public Dictionary<int, Account?> ServiceCreatorAccounts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PriceFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public int RateFilter { get; set; }

        [HttpGet("ListServices")]
        public async Task<IActionResult> Services(int PriceFilter, int RateFilter)
        {
            int page = 1;

            var response = await client.GetAsync($"{ServiceApiUrl}/available");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var services = JsonSerializer.Deserialize<IEnumerable<Service>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Lọc giá trước
                if (PriceFilter == 1)
                {
                    services = services.OrderBy(x => x.Price);
                }
                else if (PriceFilter == 2)
                {
                    services = services.OrderByDescending(x => x.Price);
                }

                // Lọc đánh giá
                if (RateFilter > 0 && RateFilter <= 5)
                {
                    int minStar = (int)Math.Round((double)RateFilter, MidpointRounding.AwayFromZero); // Làm tròn giống như hiển thị sao
                    int maxStar = minStar + 1;

                    if (minStar == 5)
                    {
                        services = services.Where(s => Math.Round(s.AverageRating ?? 0, MidpointRounding.AwayFromZero) == 5);
                    }
                    else
                    {
                        services = services.Where(s =>
                            Math.Round(s.AverageRating ?? 0, MidpointRounding.AwayFromZero) == minStar);
                    }
                }


                // Tính tổng số trang
                int totalServices = services.Count();
                int totalPage = (int)Math.Ceiling(totalServices / (double)PageSize);

                // Thiết lập trang hiện tại
                CurrentPage = page;

                // Lấy các dịch vụ đã khai báo cho trang hiện tại
                ServiceList = services.Skip((page - 1) * PageSize).Take(PageSize);

                // Khởi tạo dictionary để lưu tài khoản của người tạo dịch vụ
                ServiceCreatorAccounts = new Dictionary<int, Account?>();

                foreach (var service in ServiceList)
                {
                    var response1 = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{service.CreatorId}");

                    if (response1.IsSuccessStatusCode) // Kiểm tra xem API có trả về thành công không
                    {
                        var content1 = await response1.Content.ReadAsStringAsync();
                        var account = JsonSerializer.Deserialize<Account>(content1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (account != null) // Đảm bảo account không bị null
                        {
                            ServiceCreatorAccounts[service.ServiceId] = account;
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Lỗi: Không thể giải mã JSON từ API cho ServiceID: {service.ServiceId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ API trả về lỗi {response1.StatusCode} cho ServiceID: {service.ServiceId}");
                    }
                }

                var viewModel = new ServiceListViewModel
                {
                    ServiceList = ServiceList,
                    ServiceCreatorAccounts = ServiceCreatorAccounts,
                    CurrentPage = page,
                    TotalPages = totalPage,
                    PriceFilter = PriceFilter,  // ✅ Truyền dữ liệu vào model
                    RateFilter = RateFilter     // ✅ Truyền dữ liệu vào model
                };

                return View("ServiceList", viewModel);
            }
            return View();
        }

        [HttpGet("ListServices/Pagination")]
        public async Task<IActionResult> ServicesPagination(int p, int priceFilter, int rateFilter)
        {
            CurrentPage = p;

            var response = await client.GetAsync($"{ServiceApiUrl}/available");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var services = JsonSerializer.Deserialize<IEnumerable<Service>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Lọc giá trước
                if (PriceFilter == 1)
                {
                    services = services.OrderBy(x => x.Price);
                }
                else if (PriceFilter == 2)
                {
                    services = services.OrderByDescending(x => x.Price);
                }

                // Lọc đánh giá
                if (RateFilter > 0 && RateFilter <= 5)
                {
                    int minStar = (int)Math.Round((double)RateFilter, MidpointRounding.AwayFromZero); // Làm tròn giống như hiển thị sao
                    int maxStar = minStar + 1;

                    if (minStar == 5)
                    {
                        services = services.Where(s => Math.Round(s.AverageRating ?? 0, MidpointRounding.AwayFromZero) == 5);
                    }
                    else
                    {
                        services = services.Where(s =>
                            Math.Round(s.AverageRating ?? 0, MidpointRounding.AwayFromZero) == minStar);
                    }
                }

                // Tính tổng số trang
                int totalServices = services.Count();
                int totalPage = (int)Math.Ceiling(totalServices / (double)PageSize);

                // Lấy các dịch vụ đã khai báo cho trang hiện tại
                ServiceList = services.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

                // Khởi tạo dictionary để lưu tài khoản của người tạo dịch vụ
                ServiceCreatorAccounts = new Dictionary<int, Account?>();

                foreach (var service in ServiceList)
                {
                    var response1 = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{service.CreatorId}");

                    if (response1.IsSuccessStatusCode) // Kiểm tra xem API có trả về thành công không
                    {
                        var content1 = await response1.Content.ReadAsStringAsync();


                        var account = JsonSerializer.Deserialize<Account>(content1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (account != null) // Đảm bảo account không bị null
                        {
                            ServiceCreatorAccounts[service.ServiceId] = account;
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Lỗi: Không thể giải mã JSON từ API cho ServiceID: {service.ServiceId}");
                        }

                    }
                    else
                    {
                        Console.WriteLine($"⚠️ API trả về lỗi {response1.StatusCode} cho ServiceID: {service.ServiceId}");
                    }
                }

                var viewModel = new ServiceListViewModel
                {
                    ServiceList = ServiceList,
                    ServiceCreatorAccounts = ServiceCreatorAccounts,
                    CurrentPage = p,
                    TotalPages = totalPage,
                    PriceFilter = PriceFilter,  // ✅ Truyền dữ liệu vào model
                    RateFilter = RateFilter     // ✅ Truyền dữ liệu vào model
                };

                return View("ServiceList", viewModel);
            }
            return View();
        }


        [BindProperty]
        public string InputRequestContent { get; set; }
        [BindProperty]
        public int InputServiceId { get; set; }
        [Authorize]
        [HttpPost("RequestService")]
        public async Task<IActionResult> OnPostRequestService()
        {
            // Lấy AccountID từ Session
            // Để tạm mốt có code r làm
            int? accountId = Convert.ToInt32(HttpContext.Session.GetInt32("AccountID"));

            int getAccId = accountId.Value;



            //int getAccId = 2;

            // Lấy thông tin tài khoản từ API
            var accountResponse = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{getAccId}");
            if (!accountResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Lỗi lấy thông tin tài khoản (HTTP {accountResponse.StatusCode})");
                return RedirectToPage("/Service/ListServices");
            }

            var accountContent = await accountResponse.Content.ReadAsStringAsync();
            var getAccount = JsonSerializer.Deserialize<Account>(accountContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (getAccount == null)
            {
                Console.WriteLine("⚠️ Không lấy được thông tin tài khoản.");
                return RedirectToPage("/Service/ListServices");
            }

            // Tạo đối tượng BookingService
            var newBooking = new
            {
                ServiceId = InputServiceId,
                BookingBy = getAccId,
                BookingAt = DateTime.UtcNow,
                BookingStatus = "sending",
                IsDeletedFarmer = false,
                Content = InputRequestContent,
                IsDeletedExpert = false
            };

            // Chuyển đổi sang JSON
            var jsonBooking = new StringContent(JsonSerializer.Serialize(newBooking), Encoding.UTF8, "application/json");

            // Gửi API POST request
            var response = await client.PostAsync(BookingApiUrl, jsonBooking);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Book lịch thất bại (HTTP {response.StatusCode})");
                return RedirectToPage("/Service/ListServices");
            }

            // Lưu session với username
            HttpContext.Session.SetString("UserSession", getAccount.Username);

            Console.WriteLine("✅ Book thành công!");

            /*return RedirectToPage("/Services/BookingSuccess");*/
            return View("BookingSuccess");
        }

        [HttpGet("BookingSuccess")]
        public IActionResult BookingSuccess()
        {
            return View(); // Trả về trang Create.cshtml
        }

        public Client.Models.Service ServiceDetail { get; set; }
        public IEnumerable<ServiceRating> ServiceRatingList { get; set; }
        public IEnumerable<ServiceRating> MoreRatingList { get; set; }
        public int CountBookingService { get; set; }
        public Account CreatorService { get; set; }
        [HttpGet("ServiceDetails/{id}")]
        public async Task<IActionResult> ServiceDetails(int id)
        {
            if (id == null)
            {
                return RedirectToPage("Services", "Services");
            }

            // Tạm không có 
            int? accountId = HttpContext.Session.GetInt32("AccountID");
            //int accountId = 2;

            if (accountId == null)
            {
                return RedirectToAction("Index", "Authen"); // Chuyển hướng chính xác // Chuyển hướng nếu chưa đăng nhập
            }

            // Lấy thông tin dịch vụ
            var serviceResponse = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{id}");
            if (serviceResponse.IsSuccessStatusCode)
            {
                var serviceJson = await serviceResponse.Content.ReadAsStringAsync();
                ServiceDetail = JsonSerializer.Deserialize<Service>(serviceJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Lấy thông tin người đăng nhập
            var accLoginResponse = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{accountId}");
            Account accLogin = null;
            if (accLoginResponse.IsSuccessStatusCode)
            {
                var accountJson = await accLoginResponse.Content.ReadAsStringAsync();
                accLogin = JsonSerializer.Deserialize<Account>(accountJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            Console.WriteLine("check pint");

            if (ServiceDetail == null)
            {
                return RedirectToPage("Services", "Services");
            }

            // 🔹 Lấy thông tin người tạo dịch vụ
            var accountResponse = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{ServiceDetail.CreatorId}");
            Account createAccSer = null;
            if (accountResponse.IsSuccessStatusCode)
            {
                var accountJson = await accountResponse.Content.ReadAsStringAsync();
                CreatorService = JsonSerializer.Deserialize<Account>(accountJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Lấy các thông tin liên quan đến Request
            var bookingResponse = await client.GetAsync($"{BookingApiUrl}/count-confirm/{ServiceDetail.ServiceId}");
            if (bookingResponse.IsSuccessStatusCode)
            {
                var bookingJson = await bookingResponse.Content.ReadAsStringAsync();
                CountBookingService = JsonSerializer.Deserialize<int>(bookingJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Lấy các thông tin đánh giá dịch vụ đó
            var ratingResponse = await client.GetAsync($"{RatingApiUrl}/all-by-serId/{ServiceDetail.ServiceId}");
            if (ratingResponse.IsSuccessStatusCode)
            {
                var ratingJson = await ratingResponse.Content.ReadAsStringAsync();
                var ratings = JsonSerializer.Deserialize<IEnumerable<ServiceRating>>(ratingJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ServiceRatingList = ratings?.OrderByDescending(r => r.RatedAt).Take(5);
            }

            var ReviewerAccounts = new Dictionary<int, Account>();
            if (ServiceRatingList != null)
            {
                foreach (var rating in ServiceRatingList)
                {
                    if (!ReviewerAccounts.ContainsKey(rating.UserId))
                    {
                        int accIdRev = rating.UserId;
                        var accountRevResponse = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{rating.UserId}");
                        if (accountRevResponse.IsSuccessStatusCode)
                        {
                            var accountJson = await accountRevResponse.Content.ReadAsStringAsync();
                            var reviewAcc = JsonSerializer.Deserialize<Account>(accountJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            ReviewerAccounts[rating.UserId] = reviewAcc;
                        }
                        else
                        {
                            ReviewerAccounts[rating.UserId] = null;
                        }
                    }
                }
            }

            // 🔹 Gửi dữ liệu tới View
            ViewData["AccountLogin"] = accLogin;
            ViewData["AccountID"] = accountId;
            ViewBag.AccountID = accountId;

            var viewModel = new ServiceListViewModel
            {
                ServiceDetail = ServiceDetail,
                InputServiceId = ServiceDetail.ServiceId,
                CreatorService = CreatorService,
                ServiceRatingList = ServiceRatingList,
                ReviewerAccounts = ReviewerAccounts
            };

            return View("ServiceDetails", viewModel);
        }

        [HttpGet("ServiceDetails/LoadMoreReview")]
        public async Task<IActionResult> LoadMoreReview(int serviceId, int skip, int take)
        {
            Console.WriteLine("Tai them");
            Console.WriteLine($"LoadMoreReview called with serviceId: {serviceId}, skip: {skip}, take: {take}");

            // Gọi API để lấy tất cả đánh giá của dịch vụ
            var ratingResponse = await client.GetAsync($"{RatingApiUrl}/all-by-serId/{serviceId}");
            if (!ratingResponse.IsSuccessStatusCode)
            {
                return BadRequest("Không thể lấy dữ liệu đánh giá.");
            }

            var ratingJson = await ratingResponse.Content.ReadAsStringAsync();
            var ratings = JsonSerializer.Deserialize<IEnumerable<ServiceRating>>(ratingJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Lọc và phân trang đánh giá
            var moreRatings = ratings?.OrderByDescending(r => r.RatedAt).Skip(skip).Take(take) ?? new List<ServiceRating>();

            // Lấy thông tin người đánh giá
            var reviewerAccounts = new Dictionary<int, Account>();
            foreach (var rating in moreRatings)
            {
                if (!reviewerAccounts.ContainsKey(rating.UserId))
                {
                    var accountRevResponse = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{rating.UserId}");
                    if (accountRevResponse.IsSuccessStatusCode)
                    {
                        var accountJson = await accountRevResponse.Content.ReadAsStringAsync();
                        var reviewAcc = JsonSerializer.Deserialize<Account>(accountJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        reviewerAccounts[rating.UserId] = reviewAcc;
                    }
                    else
                    {
                        reviewerAccounts[rating.UserId] = null;
                    }
                }
            }

            // Trả về JSON thay vì PartialView (dành cho API)
            return Ok(new
            {
                Ratings = moreRatings,
                Reviewers = reviewerAccounts
            });
        }


        [BindProperty]
        public decimal RatingPoint { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Cần bạn đóng góp ý kiến")]
        public string CommentService { get; set; }
        [HttpPost("ReviewService")]
        public async Task<IActionResult> ReviewService()
        {
            if (RatingPoint == 0)
            {
                TempData["NoRating"] = "Số sao không được để trống";
                return RedirectToPage("/Services/ServiceDetails", new { id = InputServiceId });
            }

            // Để sau
            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("AccountID"));
            //int userId = 2;

            // 🔹 Tạo đánh giá mới
            var newRating = new
            {
                ServiceId = InputServiceId,
                UserId = userId,
                Rating = RatingPoint,
                Comment = CommentService,
                RatedAt = DateTime.Now
            };

            var ratingJson = new StringContent(JsonSerializer.Serialize(newRating), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{RatingApiUrl}", ratingJson);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Lỗi tạo đánh giá (HTTP {response.StatusCode})");
                return RedirectToAction("ServiceDetails", "Services", new { id = InputServiceId });
            }

            // 🔹 Lấy danh sách đánh giá để tính AverageRating
            var ratingListResponse = await client.GetAsync($"{RatingApiUrl}/all-by-serId/{InputServiceId}");
            if (!ratingListResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Không thể lấy danh sách đánh giá (HTTP {ratingListResponse.StatusCode})");
                return RedirectToAction("ServiceDetails", "Services", new { id = InputServiceId });
            }

            var ratingListJson = await ratingListResponse.Content.ReadAsStringAsync();
            var ratings = JsonSerializer.Deserialize<IEnumerable<ServiceRating>>(ratingListJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (ratings == null || !ratings.Any())
            {
                Console.WriteLine("⚠️ Không có đánh giá nào.");
                return RedirectToAction("ServiceDetails", "Services", new { id = InputServiceId });
            }

            // 🔹 Tính toán lại AverageRating
            decimal sumRate = ratings.Sum(r => r.Rating);
            decimal avgRate = Math.Round(sumRate / ratings.Count(), 1);

            // 🔹 Lấy dịch vụ cần nhập nhật
            var serviceUpdateResponse = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{InputServiceId}");
            if (!serviceUpdateResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Không thể lấy service cần cập nhật {serviceUpdateResponse.StatusCode})");
                return RedirectToAction("ServiceDetails", "Services", new { id = InputServiceId });
            }
            var serviceUpdateContent = await serviceUpdateResponse.Content.ReadAsStringAsync();
            var getServiceUpdate = JsonSerializer.Deserialize<Service>(serviceUpdateContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            getServiceUpdate.RatingCount += 1;
            getServiceUpdate.AverageRating = avgRate;

            // 🔹 Gửi yêu cầu cập nhật AverageRating cho dịch vụ
            var updateContent = new StringContent(JsonSerializer.Serialize(getServiceUpdate), Encoding.UTF8, "application/json");

            var updateResponse = await client.PutAsync($"{ServiceApiUrl}/update/{InputServiceId}", updateContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Lỗi cập nhật rating của dịch vụ (HTTP {updateResponse.StatusCode})");
            }

            //return RedirectToPage("/Services/ServiceDetails", new { id = InputServiceId });
            return RedirectToAction("ServiceDetails", "Services", new { id = InputServiceId });

        }

        [HttpGet("ListServiceOfExpert")]
        public async Task<IActionResult> ServicesOfExperts()
        {
            int getAccId = Convert.ToInt32(HttpContext.Session.GetInt32("AccountID"));

            var response = await client.GetAsync($"{ServiceApiUrl}/all-by-accId/{getAccId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var serviceList = JsonSerializer.Deserialize<IEnumerable<Service>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				var viewModel = new ServiceListViewModel
				{
					ServiceList = serviceList // Danh sách dịch vụ
				};

				return View(viewModel);
            }
            else
            {
                Console.WriteLine($"⚠️ API trả về lỗi {response.StatusCode} khi lấy danh sách dịch vụ của tài khoản {getAccId}");
                return View(); // Trả về danh sách rỗng nếu lỗi
            }
        }

        [HttpGet("UpdateServices/{id}")]
        public async Task<IActionResult> UpdateServices(int id)
        {
            var response = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // Nếu không tìm thấy dịch vụ, trả về 404
            }

            var content = await response.Content.ReadAsStringAsync();
            var ownServiceDetail = JsonSerializer.Deserialize<Service>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // 🔹 Lấy danh sách thể loại dịch vụ
            var cateServiceResponse = await client.GetAsync($"{CateServiceUrl}");
            var serCateList = new List<CategoryService>();

            if (cateServiceResponse.IsSuccessStatusCode)
            {
                var cateContent = await cateServiceResponse.Content.ReadAsStringAsync();
                serCateList = JsonSerializer.Deserialize<List<CategoryService>>(cateContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var getCateSerResponse = await client.GetAsync($"{CateServiceUrl}/{ownServiceDetail.CategoryServiceId}");
            if (!getCateSerResponse.IsSuccessStatusCode)
            {
                return NotFound(); // Nếu không tìm thấy dịch vụ, trả về 404
            }

            var cateSerContent = await getCateSerResponse.Content.ReadAsStringAsync();
            var getCateSerId = JsonSerializer.Deserialize<CategoryService>(cateSerContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // 🔹 Chuyển dữ liệu sang ViewModel
            var viewModel = new ServiceListViewModel
            {
                ServiceDetail = ownServiceDetail,
                TitleInput = ownServiceDetail.Title,
                PriceInput = ownServiceDetail.Price,
                Description = ownServiceDetail.Content,
                SelectedCategoryServiceId = ownServiceDetail.CategoryServiceId,
                SearchCateSerId = getCateSerId,
                ServiceLCateList = serCateList
            };

            return View(viewModel);
        }

        [BindProperty]
        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(200, ErrorMessage = "Quá 200 ký tự")]
        public string TitleInput { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Không được để trống")]
        public double PriceInput { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Không được để trống")]
        public string Description { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Vui lòng chọn thể loại dịch vụ.")]
        public int? SelectedCategoryServiceId { get; set; }
        [HttpPost("UpdateServices/{id}")]
        public async Task<IActionResult> UpdateServices(ServiceListViewModel vm)
        {
            // Xóa các lỗi không mong muốn
            ModelState.Remove("Account");
            ModelState.Remove("ServiceForm");
            ModelState.Remove("CommentService");
            ModelState.Remove("MoreRatingList");
            ModelState.Remove("ReviewerAccounts");
            ModelState.Remove("ServiceRatingList");
            ModelState.Remove("InputRequestContent");
            // 🔹 Loại bỏ các trường không cần validation
            ModelState.Remove("ServiceDetail");
            ModelState.Remove("ServiceDetail.Title");
            ModelState.Remove("ServiceDetail.Content");
            ModelState.Remove("ServiceDetail.CategoryService");
            ModelState.Remove("SearchCateSerId");

            int checkCreateId = vm.ServiceDetail.CreatorId;

            int getAccId = Convert.ToInt32(HttpContext.Session.GetInt32("AccountID"));

            var checkService = vm.ServiceDetail;

            var serviceItem = new Service
            {
                ServiceId = vm.ServiceDetail.ServiceId,
                CreatorId = getAccId,
                Title = TitleInput,
                Price = PriceInput,
                Content = Description,
                CategoryServiceId = SelectedCategoryServiceId ?? 0,
                UpdatedAt = DateTime.Now,
                IsEnable = true,
                AverageRating = vm.ServiceDetail.AverageRating,
                RatingCount = vm.ServiceDetail.RatingCount
            };

            if (serviceItem.Price <= 0)
            {
                ModelState.AddModelError("PriceNotZero", "Giá phải lớn hơn 0");
            }
            else if (serviceItem.Price % 1 != 0)
            {
                Console.WriteLine("In lỗi");
                ModelState.AddModelError("PriceInteger", "Giá phải là số nguyên.");
            }

            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(serviceItem), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{ServiceApiUrl}/update/{serviceItem.ServiceId}", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ Cập nhật thành công: {serviceItem.Title}");
                    return RedirectToAction("ServicesOfExperts", "Services");
                }
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState không hợp lệ:");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"🔹 {key}: {error.ErrorMessage}");
                    }
                }
            }

            var cateServiceResponse = await client.GetAsync($"{CateServiceUrl}");
            var getCateSerList = new List<CategoryService>();

            if (cateServiceResponse.IsSuccessStatusCode)
            {
                var cateContent = await cateServiceResponse.Content.ReadAsStringAsync();
                getCateSerList = JsonSerializer.Deserialize<List<CategoryService>>(cateContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // Thêm từ đây
            var getSerResponse = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{vm.ServiceDetail.ServiceId}");
            if (!getSerResponse.IsSuccessStatusCode)
            {
                return NotFound(); // Nếu không tìm thấy dịch vụ, trả về 404
            }

            var content = await getSerResponse.Content.ReadAsStringAsync();
            var ownServiceDetail = JsonSerializer.Deserialize<Service>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var getCateSerResponse = await client.GetAsync($"{CateServiceUrl}/{SelectedCategoryServiceId}");
            if (!getCateSerResponse.IsSuccessStatusCode)
            {
                return NotFound(); // Nếu không tìm thấy dịch vụ, trả về 404
            }

            var cateSerContent = await getCateSerResponse.Content.ReadAsStringAsync();
            var getCateSerId = JsonSerializer.Deserialize<CategoryService>(cateSerContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var viewModel = new ServiceListViewModel
            {
                ServiceDetail = ownServiceDetail,
                TitleInput = TitleInput,
                PriceInput = PriceInput,
                Description = Description,
                SelectedCategoryServiceId = SelectedCategoryServiceId,
                SearchCateSerId = getCateSerId,
                ServiceLCateList = getCateSerList
            };

            return View(viewModel); // 🔹 Trả lại ViewModel thay vì `service`
        }

        [HttpGet("DisableService/{id}")]
        public async Task<IActionResult> DisableService(int id)
        {
            var response = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Đổi trạng thái thất bại: Không tìm thấy dịch vụ");
				return RedirectToAction("ServicesOfExperts", "Services");
			}

            var content = await response.Content.ReadAsStringAsync();
            var service = JsonSerializer.Deserialize<Service>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            service.IsEnable = false;

            var updateResponse = await client.PutAsJsonAsync($"{ServiceApiUrl}/update/{id}", service);

            if (!updateResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Cập nhật trạng thái dịch vụ thất bại");
                return View(service);
            }

            return RedirectToAction("ServicesOfExperts", "Services");
        }

		[HttpGet("EnableService/{id}")]
		public async Task<IActionResult> EnableService(int id)
        {
            var response = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{id}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Đổi trạng thái thất bại: Không tìm thấy dịch vụ");
                return RedirectToPage("/Service/ListServiceExpert");
            }

            var content = await response.Content.ReadAsStringAsync();
            var service = JsonSerializer.Deserialize<Service>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (service == null)
            {
                Console.WriteLine("Đổi trạng thái thất bại: Dịch vụ null");
				return RedirectToAction("ServicesOfExperts", "Services");
			}

            service.IsEnable = true;

            var updateResponse = await client.PutAsJsonAsync($"{ServiceApiUrl}/update/{id}", service);

            if (!updateResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Cập nhật trạng thái dịch vụ thất bại");
            }

			return RedirectToAction("ServicesOfExperts", "Services");
		}

        // Xóa dịch vụ phía
        [HttpGet("DeleteServices/{id}")]
        public async Task<IActionResult> DeleteServices(int id)
        {
            var deleteResponse = await client.PutAsync($"{ServiceApiUrl}/soft-delete/{id}", null);

            if (!deleteResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Xóa dịch vụ thất bại!");
                TempData["ErrorMessage"] = "Không thể xóa dịch vụ. Vui lòng thử lại!";
				return RedirectToAction("ServicesOfExperts", "Services");
			}

            Console.WriteLine($"Dịch vụ {id} đã được xóa mềm thành công!");
            TempData["SuccessMessage"] = "Dịch vụ đã được xóa thành công!";

			return RedirectToAction("ServicesOfExperts", "Services");
		}

		public IEnumerable<Client.Models.CategoryService> SerCateList { get; set; }
		[HttpGet("CreateServices")]
        public async Task<IActionResult> CreateServices()
        {
            var cateServiceResponse = await client.GetAsync($"{CateServiceUrl}");

            if (true)
            {
				var content = await cateServiceResponse.Content.ReadAsStringAsync();
				SerCateList = JsonSerializer.Deserialize<IEnumerable<CategoryService>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
			}

            var viewModel = new ServiceListViewModel
            {
                ServiceLCateList = SerCateList
            };
			return View(viewModel);
        }

		
        [HttpPost("CreateServices")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateServices(ServiceListViewModel vm)
        {
            // Xóa các lỗi không mong muốn
            ModelState.Remove("Account");
            ModelState.Remove("ServiceForm");
            ModelState.Remove("CommentService");
            ModelState.Remove("MoreRatingList");
            ModelState.Remove("ReviewerAccounts");
            ModelState.Remove("ServiceRatingList");
            ModelState.Remove("InputRequestContent");
            // 🔹 Loại bỏ các trường không cần validation
            ModelState.Remove("ServiceDetail");
            ModelState.Remove("ServiceDetail.Title");
            ModelState.Remove("ServiceDetail.Content");
            ModelState.Remove("ServiceDetail.CategoryService");
            ModelState.Remove("SearchCateSerId");

            int getAccId = Convert.ToInt32(HttpContext.Session.GetInt32("AccountID"));

            var service = new Service
            {
                CreatorId = getAccId,
                Title = vm.TitleInput,
                Price = vm.PriceInput,
                IsEnable = true,
                Content = vm.Description,
                CategoryServiceId = vm.SelectedCategoryServiceId ?? 0 // 🔹 Nhận giá trị từ ViewModel
            };

            // Kiểm tra giá trị của CategoryServiceId
            Console.WriteLine($"CategoryServiceId nhận được: {service.CategoryServiceId}");

            if (service.Price <= 0)
            {
                ModelState.AddModelError("PriceNotZero", "Giá phải lớn hơn 0");
            }
            else if (service.Price % 1 != 0)
            {
                ModelState.AddModelError("PriceInteger", "Giá phải là số nguyên.");
            }


            if (ModelState.IsValid)
            {
                var createJson = new StringContent(JsonSerializer.Serialize(service), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ServiceApiUrl, createJson);

                string errorResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"⚠️ Lỗi từ server: {response.StatusCode} - Nội dung lỗi: {errorResponse}");


                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ServicesOfExperts", "Services");
                }

                Console.WriteLine($"Response lỗi: {response.StatusCode}");
                ModelState.AddModelError("", "Không thể tạo. Vui lòng thử lại.");
            }

            var cateServiceResponse = await client.GetAsync($"{CateServiceUrl}");

            if (true)
            {
                var content = await cateServiceResponse.Content.ReadAsStringAsync();
                vm.ServiceLCateList = JsonSerializer.Deserialize<IEnumerable<CategoryService>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return View(vm); // 🔹 Trả lại ViewModel thay vì `service`
        }

        /*[HttpGet]
        public async Task<IActionResult> IndexAvailable(int? p, int? priceFilter, int? rateFilter)
        {
            // Gọi API lấy danh sách dịch vụ khả dụng
            var response = await client.GetAsync($"{ServiceApiUrl}/available");
            if (!response.IsSuccessStatusCode)
            {
                return View(new ServiceListViewModel()); // Trả về view rỗng nếu lỗi
            }

            var content = await response.Content.ReadAsStringAsync();
            var services = JsonSerializer.Deserialize<IEnumerable<Service>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Thực hiện lọc theo giá hoặc đánh giá nếu có
            if (priceFilter == 1) services = services.OrderBy(s => s.Price);
            if (priceFilter == 2) services = services.OrderByDescending(s => s.Price);
            if (rateFilter.HasValue) services = services.Where(s => (int)Math.Floor(s.AverageRating ?? 0) == rateFilter.Value);

            // Phân trang
            int pageSize = 10; // Số dịch vụ mỗi trang
            int pageNumber = p ?? 1;
            int totalPages = (int)Math.Ceiling((double)services.Count() / pageSize);

            var pagedServices = services.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            // Trả về model dữ liệu cho Razor View
            var viewModel = new ServiceListViewModel
            {
                ServiceList = pagedServices,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                PriceFilter = priceFilter,
                RateFilter = rateFilter
            };

            return View(viewModel);
        }*/
    }
}
