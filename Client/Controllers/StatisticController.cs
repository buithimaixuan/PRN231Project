using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class StatisticController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
