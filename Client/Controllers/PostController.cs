using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Client.Models;
using Client.DTOs;
using Client.ViewModel;
using Microsoft.Extensions.Internal;
using Client.DTOs.Request;
using Azure;

namespace Client.Controllers
{
    public class PostController : Controller
    {
        private readonly HttpClient client;
        private readonly string postUrl;
        private readonly string categoryPostUrl;
        private readonly string postImageUrl;
        private readonly string accountUrl;
        private readonly string cloudUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostController(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            postUrl = "https://localhost:7231/api/post";
            categoryPostUrl = "https://localhost:7231/api/post-categories";
            postImageUrl = "https://localhost:7231/api/image";
            accountUrl = "https://localhost:7272/api/Accounts";
            cloudUrl = "https://localhost:7231/api/cloudinary";
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([Bind(Prefix = "PostViewModel")] PostViewModel request)
        {
            if (request.AccountId == null || request.AccountId <= 0)
            {
                return RedirectToAction("Index", "Authen");
            }

            if (request.CategoryId == null || request.CategoryId <= 0)
            {
                request.CategoryId = 1;
            }

            PostRequestDTO postRequest = new PostRequestDTO();
            postRequest.CategoryPostId = request.CategoryId;
            postRequest.AccountId = request.AccountId;
            postRequest.PostContent = request.ContentPost;

            var contentPost = new StringContent(JsonConvert.SerializeObject(postRequest), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage responsePost = await client.PostAsync($"{postUrl}", contentPost);

            //NẾU TẠO POST KHÔNG THÀNH CÔNG
            if(!responsePost.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }

            var responseString = await responsePost.Content.ReadAsStringAsync();
            Post postResponse = JsonConvert.DeserializeObject<Post>(responseString);

            if (postResponse == null)
            {
                Console.WriteLine("Tạo bài viết thất bại!");
            }

            int postId = postResponse.PostId;

            if (request.Images != null && request.Images.Count > 0)
            {
                //Cấu hình content gửi về api
                using (var contentCloud = new MultipartFormDataContent())
                {
                    foreach (var image in request.Images)
                    {
                        var memoryStream = new MemoryStream();
                        image.OpenReadStream().CopyTo(memoryStream);
                        memoryStream.Position = 0; // Đặt lại vị trí đầu stream

                        var fileContent = new StreamContent(memoryStream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);

                        contentCloud.Add(fileContent, "request", image.FileName);
                    }

                    //Gọi api
                    var responseCloud = await client.PostAsync($"{cloudUrl}/upload-list", contentCloud);

                    if (responseCloud.IsSuccessStatusCode)
                    {
                        var responseStringImage = await responseCloud.Content.ReadAsStringAsync();
                        List<ImageUploadResponseDTO> imageResponse = JsonConvert.DeserializeObject<List<ImageUploadResponseDTO>>(responseStringImage);

                        if (imageResponse == null)
                        {
                            Console.WriteLine("Upload image không thành công.");
                        } else
                        {
                            foreach (var image in imageResponse)
                            {
                                PostImageRequestDTO imageRequest = new PostImageRequestDTO();
                                imageRequest.PostId = postId;
                                imageRequest.ImageUrl = image.ImageUrl;
                                imageRequest.IsDeleted = false;
                                imageRequest.PublicId = image.PublicId;

                                var contentImage = new StringContent(JsonConvert.SerializeObject(imageRequest), System.Text.Encoding.UTF8, "application/json");
                                var responseImageApi = await client.PostAsync(postImageUrl, contentImage);
                            }
                        }
                            
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> PostDetail(int id)
        {
            if (GetCookiesAccId() <= 0)
            {
                return RedirectToAction("Index", "Authen");
            }
            try
            {
                // Lấy chi tiết bài đăng
                Console.WriteLine($"Calling API: {postUrl}/{id}");
                HttpResponseMessage postResponse = await client.GetAsync($"{postUrl}/{id}");
                if (!postResponse.IsSuccessStatusCode)
                {
                    string errorMessage = await postResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error from API: {errorMessage}");
                    ViewBag.Error = $"Không thể tải bài đăng: {errorMessage}";
                    return View(new PostDetailViewModel());
                }
                string postData = await postResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Raw data from API: {postData}");
                var postDTO = JsonConvert.DeserializeObject<PostDTO>(postData);
                if (postDTO == null)
                {
                    Console.WriteLine("Deserialize failed: postDTO is null.");
                    ViewBag.Error = "Không thể deserialize dữ liệu bài đăng.";
                    return View(new PostDetailViewModel());
                }

                // Lấy thông tin Account (nếu AccountId có giá trị)
                Account account = null;
                if (postDTO.post.AccountId.HasValue)
                {
                    Console.WriteLine($"{accountUrl}/{postDTO.post.AccountId}");
                    HttpResponseMessage accountResponse = await client.GetAsync($"{accountUrl}/DetailFarmer{postDTO.post.AccountId.Value}");
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        string accountData = await accountResponse.Content.ReadAsStringAsync();
                        account = JsonConvert.DeserializeObject<Account>(accountData);
                    }
                }
                postDTO.Account = account;

                // Lấy thông tin danh mục
                HttpResponseMessage categoryResponse = await client.GetAsync($"{categoryPostUrl}/{postDTO.post.CategoryPostId}");
                CategoryPost categoryPost = null;
                if (categoryResponse.IsSuccessStatusCode)
                {
                    string categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    categoryPost = JsonConvert.DeserializeObject<CategoryPost>(categoryData);
                }

                // Lấy số lượng lượt thích
                HttpResponseMessage likeResponse = await client.GetAsync($"{postUrl}/all-likes/{id}");
                List<LikePost> likes = new List<LikePost>();
                int likeCount = 0;
                if (likeResponse.IsSuccessStatusCode)
                {
                    string likeData = await likeResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Like API Response: {likeData}");
                    var likeResult = JsonConvert.DeserializeObject<LikeResponse>(likeData);

                    if (likeResult != null && likeResult.Likes != null)
                    {
                        likes = likeResult.Likes;
                        likeCount = likes.Count;
                    }
                    else
                    {
                        Console.WriteLine("Likes data is null or missing in the API response.");
                    }
                }
                else
                {
                    Console.WriteLine($"Like API failed: {await likeResponse.Content.ReadAsStringAsync()}");
                }

                // Lấy số lượng bình luận và danh sách bình luận
                HttpResponseMessage commentResponse = await client.GetAsync($"{postUrl}/comments/{id}");
                List<Comment> comments = new List<Comment>();
                int commentCount = 0;
                if (commentResponse.IsSuccessStatusCode)
                {
                    string commentData = await commentResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Comment API Response: {commentData}");
                    var commentResult = JsonConvert.DeserializeObject<CommentResponse>(commentData);

                    if (commentResult != null)
                    {
                        commentCount = commentResult.CommentCount;
                        comments = commentResult.Comments ?? new List<Comment>(); // Fallback to empty list if null
                    }
                    else
                    {
                        Console.WriteLine("Comment API response could not be deserialized.");
                    }
                }
                else
                {
                    Console.WriteLine($"Comment API failed: {await commentResponse.Content.ReadAsStringAsync()}");
                }

                // Lấy số lượng chia sẻ (từ postDTO.sharePosts)
                int shareCount = postDTO.sharePosts?.Count() ?? 0;

                // Lấy danh sách URL hình ảnh
                List<string> postImageUrls = postDTO.postImages?.Select(pi => pi.ImageUrl).ToList() ?? new List<string>();

                // Kiểm tra xem người dùng hiện tại có thích bài đăng không (giả sử bạn có thông tin người dùng đăng nhập)
                bool isLikedByUser = false; // Cần logic để kiểm tra, ví dụ: so sánh với AccountId của người dùng hiện tại

                // Tạo ViewModel
                var viewModel = new PostDetailViewModel
                {
                    PostDTO = postDTO,
                    ListComment = comments,
                    CommentAccounts = new Dictionary<int?, Account>(), // Cần lấy thông tin Account cho từng bình luận nếu muốn hiển thị
                    CountCommentPost = commentCount,
                    CountLikePost = likeCount,
                    CountSharePost = shareCount,
                    IsLikedByUser = isLikedByUser,
                    View = 100, // Giá trị giả lập, bạn có thể lấy từ API nếu có
                    CommentContent = "",
                    CategoryPost = categoryPost,
                    PostImageUrls = postImageUrls
                };

                // Lấy thông tin Account cho từng bình luận
                foreach (var comment in viewModel.ListComment)
                {
                    if (comment.AccountId != null) // Chỉ kiểm tra null
                    {
                        HttpResponseMessage commentAccountResponse = await client.GetAsync($"{accountUrl}/DetailFarmer{comment.AccountId}");
                        if (commentAccountResponse.IsSuccessStatusCode)
                        {
                            string commentAccountData = await commentAccountResponse.Content.ReadAsStringAsync();
                            var commentAccount = JsonConvert.DeserializeObject<Account>(commentAccountData);
                            if (commentAccount != null)
                            {
                                viewModel.CommentAccounts[comment.AccountId] = commentAccount;
                            }
                        }
                    }
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                return View(new PostDetailViewModel());
            }
        }
        public class LikeResponse
        {
            public List<LikePost> Likes { get; set; }
        }

        public class CommentResponse
        {
            public int CommentCount { get; set; }
            public List<Comment> Comments { get; set; }
        }


        public int GetCookiesAccId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity.IsAuthenticated)
            {
                return -1; // Người dùng chưa đăng nhập
            }

            var accountIdClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == "AccountID");
            if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out int accountId))
            {
                return accountId;
            }

            return -1; // Không tìm thấy AccountID
        }
    }
}