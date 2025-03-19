using System;
using System.Collections.Generic;
namespace Client.Models
{
    public partial class CategoryNews
    {
        public int CategoryNewsId { get; set; }

        public string CategoryNewsName { get; set; } = null!;

        public string? CategoryNewsDescription { get; set; }

        public virtual ICollection<News> News { get; set; } = new List<News>();
    }
}


