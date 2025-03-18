﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure;
using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ServicesController : Controller
    {
        private readonly HttpClient client = null;
        private string ServiceApiUrl = "";
        private string AccountApiUrl = "";
        private string BookingApiUrl = "";
        private string RatingApiUrl = "";
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


        [HttpGet]
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
                if (RateFilter > 0 && RateFilter < 5)
                {
                    services = services.Where(s => s.AverageRating >= RateFilter && s.AverageRating < (RateFilter + 1));
                }
                else if (RateFilter == 5)
                {
                    services = services.Where(s => s.AverageRating == RateFilter);
                }

                // Tính tổng số trang
                int totalServices = services.Count();
                TotalPages = (int)Math.Ceiling(totalServices / (double)PageSize);

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

                        if (!string.IsNullOrWhiteSpace(content1)) // Kiểm tra nếu content không rỗng
                        {
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
                            Console.WriteLine($"⚠️ Lỗi: Phản hồi rỗng từ API cho ServiceID: {service.ServiceId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ API trả về lỗi {response1.StatusCode} cho ServiceID: {service.ServiceId}");
                    }
                }


                return View(services);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ServicesPagination(int p, int priceFilter, int rateFilter)
        {
            CurrentPage = p;

            var response = await client.GetAsync($"{ServiceApiUrl}/available");
            if (response.IsSuccessStatusCode) {
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
                if (RateFilter > 0 && RateFilter < 5)
                {
                    services = services.Where(s => s.AverageRating >= RateFilter && s.AverageRating < (RateFilter + 1));
                }
                else if (RateFilter == 5)
                {
                    services = services.Where(s => s.AverageRating == RateFilter);
                }

                // Tính tổng dịch vụ

                var respone1 = await client.GetAsync($"{ServiceApiUrl}/count-all");

                if (!respone1.IsSuccessStatusCode)
                {
                    var content1 = await respone1.Content.ReadAsStringAsync();
                    int totalServices = JsonSerializer.Deserialize<int>(content1, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    TotalPages = (int)Math.Ceiling(totalServices / (double)PageSize);
                }

                ServiceList = services.Skip((CurrentPage - 1) * PageSize).Take(PageSize);

                ServiceCreatorAccounts = new Dictionary<int, Account?>();

                foreach (var service in ServiceList)
                {
                    var response1 = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{service.CreatorId}");

                    if (response1.IsSuccessStatusCode) // Kiểm tra xem API có trả về thành công không
                    {
                        var content1 = await response1.Content.ReadAsStringAsync();

                        if (!string.IsNullOrWhiteSpace(content1)) // Kiểm tra nếu content không rỗng
                        {
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
                            Console.WriteLine($"⚠️ Lỗi: Phản hồi rỗng từ API cho ServiceID: {service.ServiceId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ API trả về lỗi {response1.StatusCode} cho ServiceID: {service.ServiceId}");
                    }
                }
                PriceFilter = priceFilter;
                RateFilter = rateFilter;

                return View();
            }
            return View();
        }

        [BindProperty]
        public string InputRequestContent { get; set; }
        [BindProperty]
        public int InputServiceId { get; set; }

        [HttpPost]
        public async Task<IActionResult> OnPostRequestService()
        {
            // Lấy AccountID từ Session
            int? accountId = HttpContext.Session.GetInt32("AccountID");
            if (accountId == null)
            {
                Console.WriteLine("⚠️ Không tìm thấy AccountID trong session.");
                return RedirectToPage("/Account/Login");
            }

            int getAccId = accountId.Value;

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
            /*var jsonBooking = JsonSerializer.Serialize(newBooking);
            var content = new StringContent(jsonBooking, Encoding.UTF8, "application/json");*/
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

            return RedirectToPage("/Services/BookingSuccess");
        }

        public Client.Models.Service ServiceDetail { get; set; }
        public IEnumerable<ServiceRating> ServiceRatingList { get; set; }
        public IEnumerable<ServiceRating> MoreRatingList { get; set; }
        public int CountBookingService { get; set; }
        public Account CreatorService { get; set; }
        [HttpGet]
        public async Task<IActionResult> ServiceDetails(int id)
        {
            if (id == null)
            {
                RedirectToPage("/Index");
            }

            var serviceResponse = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{id}");
            if (!serviceResponse.IsSuccessStatusCode)
            {
                var serviceJson = await serviceResponse.Content.ReadAsStringAsync();
                ServiceDetail = JsonSerializer.Deserialize<Service>(serviceJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (ServiceDetail == null)
            {
                return RedirectToPage("/Index");
            }

            // 🔹 Lấy thông tin người tạo dịch vụ
            var accountResponse = await client.GetAsync($"{AccountApiUrl}/DetailFarmer{ServiceDetail.CreatorId}");
            if (accountResponse.IsSuccessStatusCode)
            {
                var accountJson = await accountResponse.Content.ReadAsStringAsync();
                CreatorService = JsonSerializer.Deserialize<Account>(accountJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var bookingResponse = await client.GetAsync($"{BookingApiUrl}/count-confirm/{ServiceDetail.ServiceId}");
            if (bookingResponse.IsSuccessStatusCode)
            {
                var bookingJson = await bookingResponse.Content.ReadAsStringAsync();
                CountBookingService = JsonSerializer.Deserialize<int>(bookingJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            var ratingResponse = await client.GetAsync($"{RatingApiUrl}/all-by-serId/{ServiceDetail.ServiceId}");
            if (ratingResponse.IsSuccessStatusCode)
            {
                var ratingJson = await ratingResponse.Content.ReadAsStringAsync();
                var ratings = JsonSerializer.Deserialize<IEnumerable<ServiceRating>>(ratingJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ServiceRatingList = ratings?.OrderByDescending(r => r.RatedAt).Take(5);
            }

            return View();
        }

        [BindProperty]
        public decimal RatingPoint { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Cần bạn đóng góp ý kiến")]
        public string CommentService { get; set; }
        [HttpPost]
        public async Task<IActionResult> CreateRateService()
        {
            if (RatingPoint == 0)
            {
                TempData["NoRating"] = "Số sao không được để trống";
                return RedirectToPage("/Services/ServiceDetails", new { id = InputServiceId });
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetInt32("AccountID"));

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
                return RedirectToPage("/Service/ServiceDetail", new { id = InputServiceId });
            }

            // 🔹 Lấy danh sách đánh giá để tính AverageRating
            var ratingListResponse = await client.GetAsync($"{RatingApiUrl}/all-by-serId/{InputServiceId}");
            if (ratingListResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Không thể lấy danh sách đánh giá (HTTP {ratingListResponse.StatusCode})");
                return RedirectToPage("/Service/ServiceDetail", new { id = InputServiceId });
            }

            var ratingListJson = await ratingListResponse.Content.ReadAsStringAsync();
            var ratings = JsonSerializer.Deserialize<IEnumerable<ServiceRating>>(ratingListJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (ratings == null || !ratings.Any())
            {
                Console.WriteLine("⚠️ Không có đánh giá nào.");
                return RedirectToPage("/Service/ServiceDetails", new { id = InputServiceId });
            }

            // 🔹 Tính toán lại AverageRating
            decimal sumRate = ratings.Sum(r => r.Rating);
            decimal avgRate = Math.Round(sumRate / ratings.Count(), 1);

            // 🔹 Lấy dịch vụ cần nhập nhật
            var serviceUpdateResponse = await client.GetAsync($"{ServiceApiUrl}/get-by-id/{InputServiceId}");
            if (!serviceUpdateResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Không thể lấy service cần cập nhật {serviceUpdateResponse.StatusCode})");
                return RedirectToPage("/Service/ServiceDetail", new { id = InputServiceId });
            }
            var serviceUpdateContent = await serviceUpdateResponse.Content.ReadAsStringAsync();
            var getServiceUpdate = JsonSerializer.Deserialize<Service>(serviceUpdateContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            getServiceUpdate.RatingCount += 1;
            getServiceUpdate.AverageRating = avgRate;

            // 🔹 Gửi yêu cầu cập nhật AverageRating cho dịch vụ
            var updateContent = new StringContent(JsonSerializer.Serialize(getServiceUpdate), Encoding.UTF8, "application/json");

            var updateResponse = await client.PutAsync($"{ServiceApiUrl}/{InputServiceId}", updateContent);

            if (!updateResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"⚠️ Lỗi cập nhật rating của dịch vụ (HTTP {updateResponse.StatusCode})");
            }

            return RedirectToPage("/Service/ServiceDetail", new { id = InputServiceId });
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
