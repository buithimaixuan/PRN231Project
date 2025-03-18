using Client.Models;

namespace Client.DTOs
{
    public class ServiceListViewModel
    {
        public IEnumerable<Service> ServiceList { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int? PriceFilter { get; set; }
        public int? RateFilter { get; set; }
    }
}
