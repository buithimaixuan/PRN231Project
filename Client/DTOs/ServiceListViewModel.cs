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

        [Required(ErrorMessage = "Please enter request")]
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
        [Required(ErrorMessage = "Need your comments")]
        public string CommentService { get; set; }
        public Dictionary<int, Account> ReviewerAccounts { get; set; }
        // Thêm sửa Service
        public Service ServiceForm { get; set; }
        [Required(ErrorMessage = "Cannot be left blank")]
        [StringLength(200, ErrorMessage = "Over 200 characters")]
        public string TitleInput { get; set; }
        [Required(ErrorMessage = "Cannot be left blank")]
        public double PriceInput { get; set; }
        [Required(ErrorMessage = "Cannot be left blank")]
        public string Description { get; set; }
		public IEnumerable<CategoryService> ServiceLCateList { get; set; } = new List<CategoryService>();
        [Required(ErrorMessage = "Please select service category")]
        public int? SelectedCategoryServiceId { get; set; }
        public CategoryService SearchCateSerId { get; set; }
	}
}
