using Client.Models;
using Client.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        private string authenUrl = "";
        private string accountUrl = "";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            authenUrl = "https://localhost:7272/api/Auth";
            accountUrl = "https://localhost:7272/api/Accounts";
            this._logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            int accId = GetSessionAccId();
            var viewModel = new HomeViewModel();

            if (accId > 0)
            {
                //Lay account tu accountId
                var accountInfor = await client.GetAsync($"{accountUrl}/DetailFarmer{accId}");
                var responseAccount = await accountInfor.Content.ReadAsStringAsync();
                viewModel.Account = JsonConvert.DeserializeObject<Account>(responseAccount);
            }

            //Xu ly lay list post o day, neu co
            
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetSessionAccId()
        {
            return HttpContext.Session.GetInt32("AccountID") ?? -1;
        }
    }
}
