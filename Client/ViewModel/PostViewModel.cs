using System.ComponentModel.DataAnnotations;

namespace Client.ViewModel
{
    public class PostViewModel
    {
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string ContentPost { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } // Để nhận nhiều ảnh từ form


        public PostViewModel(int accountId, string contentPost, int categoryId, List<IFormFile> images)
        {
            AccountId = accountId;
            this.ContentPost = contentPost;
            this.CategoryId = categoryId;
            Images = images;
        }

        public PostViewModel()
        {
        }
    }
}
