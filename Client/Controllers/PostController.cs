using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Client.Models;
using Client.DTOs;
using Client.ViewModel;
using Microsoft.Extensions.Internal;

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

        public PostController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            postUrl = "https://localhost:7231/api/post";
            categoryPostUrl = "https://localhost:7231/api/post-categories";
            postImageUrl = "https://localhost:7231/api/image";
            accountUrl = "https://localhost:7272/api/Accounts";
            cloudUrl = "https://localhost:7231/api/cloudinary";
        }

        public async Task<IActionResult> PostDetail(int id)
        {
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
    }
}