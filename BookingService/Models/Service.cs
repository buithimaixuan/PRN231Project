using System;
using System.Collections.Generic;

namespace BookingService.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public int CreatorId { get; set; }

    public int CategoryServiceId { get; set; }

    public DateOnly CreateAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public DateOnly? DeletedAt { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public double Price { get; set; }

    public bool? IsEnable { get; set; }

    public bool? IsDeleted { get; set; }

    public decimal? AverageRating { get; set; }

    public int? RatingCount { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual CategoryService CategoryService { get; set; } = null!;

    public virtual ICollection<ServiceRating> ServiceRatings { get; set; } = new List<ServiceRating>();
}
