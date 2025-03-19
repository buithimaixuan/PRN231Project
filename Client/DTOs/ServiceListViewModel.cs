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
    }
}
