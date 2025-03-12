using System;
using System.Collections.Generic;

namespace BookingService.Models;

public partial class CategoryService
{
    public int CategoryServiceId { get; set; }

    public string CategoryServiceName { get; set; } = null!;

    public string? CategoryServiceDescription { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
