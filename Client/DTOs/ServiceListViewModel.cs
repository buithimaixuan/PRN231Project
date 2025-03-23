using Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class ServiceListViewModel
    {
        /*public IEnumerable<Service> ServiceList { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? PriceFilter { get; set; }
        public int? RateFilter { get; set; }*/

        // _Layout
        public Account Account { get; set; }
        //
        public IEnumerable<Service> ServiceList { get; set; } = new List<Service>();
        public Dictionary<int, Account?> ServiceCreatorAccounts { get; set; } = new Dictionary<int, Account?>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        // Thêm thuộc tính để nhận dữ liệu từ form
        [Required]
        public int InputServiceId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập yêu cầu")]
        public string InputRequestContent { get; set; } = string.Empty;


        [BindProperty(SupportsGet = true)]
        public int PriceFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int RateFilter { get; set; }

        // Các thuộc tính của trang ServiceDetails
        public Service? ServiceDetail { get; set; }
        public Account? CreatorService { get; set; }
        public IEnumerable<ServiceRating> ServiceRatingList { get; set; }
        public IEnumerable<ServiceRating> MoreRatingList { get; set; }
        public int CountBookingService { get; set; }
        [BindProperty]
        public decimal RatingPoint { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Cần bạn đóng góp ý kiến")]
        public string CommentService { get; set; }
        public Dictionary<int, Account> ReviewerAccounts { get; set; }
        // Thêm sửa Service
        public Service ServiceForm { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        [StringLength(200, ErrorMessage = "Quá 200 ký tự")]
        public string TitleInput { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public double PriceInput { get; set; }
        [Required(ErrorMessage = "Không được để trống")]
        public string Description { get; set; }
		public IEnumerable<CategoryService> ServiceLCateList { get; set; } = new List<CategoryService>();
        [Required(ErrorMessage = "Vui lòng chọn thể loại dịch vụ.")]
        public int? SelectedCategoryServiceId { get; set; }
        public CategoryService SearchCateSerId { get; set; }
	}
}
