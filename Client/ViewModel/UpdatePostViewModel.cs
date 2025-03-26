using System.ComponentModel.DataAnnotations;

namespace Client.ViewModel
{
    public class UpdatePostViewModel
    {
        //DÙNG CHO BINDING DỮ LIỆU MỚI, THÊM TRƯỜNG IsDeleteOldImage
        public int PostId { get; set; }
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string ContentPost { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } // Để nhận nhiều ảnh từ form
        public bool IsDeleteOldImage { get; set; }
    }
}
