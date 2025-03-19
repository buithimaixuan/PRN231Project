using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class PostController : Controller
    {
        private readonly HttpClient client;

        private string postUrl = "";
        private string categoryPostUrl = "";
        private string postImageUrl = "";
        private string accountUrl = "";
        private string cloudUrl = "";

        public PostController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            postUrl = "http://localhost:5007/api/post";
            categoryPostUrl = "http://localhost:5007/api/post-categories";
            postImageUrl = "http://localhost:5007/api/image";
            accountUrl = "http://localhost:5157/api/Accounts";
            cloudUrl = "http://localhost:5007/api/cloudinary";
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
