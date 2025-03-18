using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using Client.DTOs;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ServicesController : Controller
    {
        private readonly HttpClient client = null;
        private string ServiceApiUrl = "";
        private string AccountApiUrl;
        private const int PageSize = 8;

        public ServicesController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ServiceApiUrl = "https://localhost:7243/api/Services";
            AccountApiUrl = "https://localhost:7272/api/Accounts";
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
        public async Task<IActionResult> IndexAvailable(int PriceFilter, int RateFilter)
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
        public async Task<IActionResult> IndexAvailablePagination(int p, int priceFilter, int rateFilter)
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
