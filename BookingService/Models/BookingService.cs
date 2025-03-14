using System;
using System.Collections.Generic;

namespace BookingService.Models;

public partial class BookingService
{
    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    public int BookingBy { get; set; }

    public DateTime BookingAt { get; set; }

    public string BookingStatus { get; set; } = null!;

    public bool? IsDeletedFarmer { get; set; }

    public bool? IsDeletedExpert { get; set; }

    public string? Content { get; set; }

    public virtual Service Service { get; set; } = null!;
}
