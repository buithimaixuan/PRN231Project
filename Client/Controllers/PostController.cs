using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Client.Models;
using Client.DTOs;
using Client.ViewModel;

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

                // Tạo ViewModel
                var viewModel = new PostDetailViewModel
                {
                    PostDTO = postDTO,
                    ListComment = new List<Comment>(),
                    CommentAccounts = new Dictionary<int, Account>(),
                    CountCommentPost = 0,
                    CountLikePost = 0,
                    CountSharePost = 0,
                    IsLikedByUser = false,
                    View = 100,
                    CommentContent = ""
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                return View(new PostDetailViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Like(int postId)
        {
            try
            {
                int currentUserId = 1;
                var like = new LikePost { AccountId = currentUserId, PostId = postId };
                var content = new StringContent(JsonConvert.SerializeObject(like), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{postUrl}/like", content);
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage likesResponse = await client.GetAsync($"{postUrl}/all-likes/{postId}");
                    int likeCount = 0;
                    if (likesResponse.IsSuccessStatusCode)
                    {
                        string likesData = await likesResponse.Content.ReadAsStringAsync();
                        var likesResult = JsonConvert.DeserializeObject<dynamic>(likesData);
                        likeCount = likesResult.LikeCount;
                    }
                    return Json(new { success = true, likeCount = likeCount });
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error from API /like: {errorMessage}");
                    return Json(new { success = false, message = $"Không thể thích bài viết: {errorMessage}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Like: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unlike(int postId)
        {
            try
            {
                int currentUserId = 1;
                HttpResponseMessage response = await client.DeleteAsync($"{postUrl}/unlike?accountId={currentUserId}&postId={postId}");
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage likesResponse = await client.GetAsync($"{postUrl}/all-likes/{postId}");
                    int likeCount = 0;
                    if (likesResponse.IsSuccessStatusCode)
                    {
                        string likesData = await likesResponse.Content.ReadAsStringAsync();
                        var likesResult = JsonConvert.DeserializeObject<dynamic>(likesData);
                        likeCount = likesResult.LikeCount;
                    }
                    return Json(new { success = true, likeCount = likeCount });
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error from API /unlike: {errorMessage}");
                    return Json(new { success = false, message = $"Không thể bỏ thích bài viết: {errorMessage}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Unlike: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int postId, string commentContent)
        {
            try
            {
                int currentUserId = 1;
                var comment = new Comment
                {
                    AccountId = currentUserId,
                    PostId = postId,
                    Content = commentContent
                };
                var content = new StringContent(JsonConvert.SerializeObject(comment), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{postUrl}/comment-on-post", content);
                if (response.IsSuccessStatusCode)
                {
                    HttpResponseMessage commentsResponse = await client.GetAsync($"{postUrl}/comments/{postId}");
                    int commentCount = 0;
                    if (commentsResponse.IsSuccessStatusCode)
                    {
                        string commentsData = await commentsResponse.Content.ReadAsStringAsync();
                        var commentsResult = JsonConvert.DeserializeObject<dynamic>(commentsData);
                        commentCount = commentsResult.CommentCount;
                    }

                    HttpResponseMessage accountResponse = await client.GetAsync($"{accountUrl}/{currentUserId}");
                    Account account = null;
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        string accountData = await accountResponse.Content.ReadAsStringAsync();
                        account = JsonConvert.DeserializeObject<Account>(accountData);
                    }

                    return Json(new
                    {
                        success = true,
                        commentCount = commentCount,
                        comment = new
                        {
                            accountId = currentUserId,
                            fullName = account?.FullName ?? "Người dùng ẩn danh",
                            avatar = account?.Avatar ?? "https://firebasestorage.googleapis.com/v0/b/prn221-69738.appspot.com/o/image%2Fuser.png?alt=media&token=e669a837-b9c8-4983-b2bd-8eb5c091d48f",
                            content = commentContent,
                            createdAt = comment.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                        }
                    });
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error from API /comment-on-post: {errorMessage}");
                    return Json(new { success = false, message = $"Không thể đăng bình luận: {errorMessage}" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in Comment: {ex.Message}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }
}