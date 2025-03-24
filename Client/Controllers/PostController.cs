using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Client.Models;
using Client.DTOs;
using Client.ViewModel;
using Microsoft.Extensions.Internal;
using System.Text;

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

                // Kiểm tra xem người dùng hiện tại có thích bài đăng không
                int? currentUserId = 1; // Replace with actual logged-in user's AccountId (e.g., from session or authentication)
                bool isLikedByUser = likes.Any(l => l.AccountId == currentUserId);

                // Tạo ViewModel
                var viewModel = new PostDetailViewModel
                {
                    PostDTO = postDTO,
                    ListComment = comments,
                    CommentAccounts = new Dictionary<int?, Account>(),
                    CountCommentPost = commentCount,
                    CountLikePost = likeCount,
                    CountSharePost = shareCount,
                    IsLikedByUser = isLikedByUser,
                    View = 100, // Giá trị giả lập, bạn có thể lấy từ API nếu có
                    CommentContent = "",
                    CategoryPost = categoryPost,
                    PostImageUrls = postImageUrls,
                    CurrentUserId = currentUserId // Pass the current user's ID to the view
                };

                // Lấy thông tin Account cho từng bình luận
                foreach (var comment in viewModel.ListComment)
                {
                    if (comment.AccountId != null)
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

        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {
            try
            {
                // Lấy currentUserId từ session hoặc authentication (thay thế giá trị cứng 1)
                int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId") ?? 1; // Giả sử bạn lưu trong session
                if (!currentUserId.HasValue)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để thích bài đăng." });
                }

                // Gọi API để thích bài đăng
                HttpResponseMessage response = await client.PostAsync(
                    $"{postUrl}/like?accountId={currentUserId}&postId={postId}", null);

                if (response.IsSuccessStatusCode)
                {
                    // Lấy lại số lượt thích (tuỳ chọn, nếu cần cập nhật chính xác)
                    HttpResponseMessage likeResponse = await client.GetAsync($"{postUrl}/all-likes/{postId}");
                    int likeCount = 0;
                    if (likeResponse.IsSuccessStatusCode)
                    {
                        string likeData = await likeResponse.Content.ReadAsStringAsync();
                        var likeResult = JsonConvert.DeserializeObject<LikeResponse>(likeData);
                        likeCount = likeResult?.Likes?.Count ?? 0;
                    }

                    return Json(new { success = true, likeCount = likeCount });
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Không thể thích bài đăng: {errorMessage}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            try
            {
                // Lấy currentUserId từ session hoặc authentication
                int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId") ?? 1; // Giả sử bạn lưu trong session
                if (!currentUserId.HasValue)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để bỏ thích bài đăng." });
                }

                // Gọi API để bỏ thích bài đăng
                HttpResponseMessage response = await client.DeleteAsync(
                    $"{postUrl}/unlike?accountId={currentUserId}&postId={postId}");

                if (response.IsSuccessStatusCode)
                {
                    // Lấy lại số lượt thích (tuỳ chọn)
                    HttpResponseMessage likeResponse = await client.GetAsync($"{postUrl}/all-likes/{postId}");
                    int likeCount = 0;
                    if (likeResponse.IsSuccessStatusCode)
                    {
                        string likeData = await likeResponse.Content.ReadAsStringAsync();
                        var likeResult = JsonConvert.DeserializeObject<LikeResponse>(likeData);
                        likeCount = likeResult?.Likes?.Count ?? 0;
                    }

                    return Json(new { success = true, likeCount = likeCount });
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Không thể bỏ thích bài đăng: {errorMessage}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            try
            {
                // Lấy currentUserId từ session hoặc authentication
                int? currentUserId = HttpContext.Session.GetInt32("CurrentUserId") ?? 1; // Thay bằng cách lấy thực tế nếu có
                if (!currentUserId.HasValue)
                {
                    return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });
                }

                if (string.IsNullOrWhiteSpace(content))
                {
                    return Json(new { success = false, message = "Nội dung bình luận không được để trống." });
                }

                // Tạo object Comment để gửi lên API
                var comment = new
                {
                    AccountId = currentUserId.Value,
                    PostId = postId,
                    Content = content
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(comment), Encoding.UTF8, "application/json");

                // Gọi API comment-on-post
                HttpResponseMessage response = await client.PostAsync($"{postUrl}/comment-on-post", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    string commentData = await response.Content.ReadAsStringAsync();
                    var newComment = JsonConvert.DeserializeObject<Comment>(commentData);

                    // Lấy thông tin tài khoản
                    string fullName = "Người dùng không xác định";
                    string avatar = "https://firebasestorage.googleapis.com/v0/b/prn221-69738.appspot.com/o/image%2Fuser.png?alt=media&token=e669a837-b9c8-4983-b2bd-8eb5c091d48f";
                    HttpResponseMessage accountResponse = await client.GetAsync($"{accountUrl}/DetailFarmer{currentUserId}");
                    if (accountResponse.IsSuccessStatusCode)
                    {
                        string accountData = await accountResponse.Content.ReadAsStringAsync();
                        var account = JsonConvert.DeserializeObject<Account>(accountData);
                        if (account != null)
                        {
                            fullName = account.FullName ?? fullName;
                            avatar = account.Avatar ?? avatar;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch account: {await accountResponse.Content.ReadAsStringAsync()}");
                    }

                    // Lấy lại số lượng bình luận
                    HttpResponseMessage commentResponse = await client.GetAsync($"{postUrl}/comments/{postId}");
                    int commentCount = 0;
                    if (commentResponse.IsSuccessStatusCode)
                    {
                        string commentsData = await commentResponse.Content.ReadAsStringAsync();
                        var commentResult = JsonConvert.DeserializeObject<CommentResponse>(commentsData);
                        commentCount = commentResult?.CommentCount ?? 0;
                    }

                    // Trả về JSON với cấu trúc rõ ràng
                    return Json(new
                    {
                        success = true,
                        comment = new
                        {
                            accountId = newComment.AccountId,
                            postId = newComment.PostId,
                            content = newComment.Content,
                            createdAt = newComment.CreatedAt.ToString("dd/MM/yyyy")
                        },
                        fullName = fullName, // Đảm bảo tên trường khớp với JS
                        avatar = avatar,     // Đảm bảo tên trường khớp với JS
                        commentCount = commentCount
                    });
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = $"Không thể thêm bình luận: {errorMessage}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
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