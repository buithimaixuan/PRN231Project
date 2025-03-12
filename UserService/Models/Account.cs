using System;
using System.Collections.Generic;

namespace UserService.Models;

public partial class Account
{
    public int AccountId { get; set; }

    public int RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public int? EmailConfirmed { get; set; }

    public string? Phone { get; set; }

    public int? PhoneConfirmed { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? ShortBio { get; set; }

    public string? EducationUrl { get; set; }

    public int? YearOfExperience { get; set; }

    public string? DegreeUrl { get; set; }

    public string? Avatar { get; set; }

    public string? Major { get; set; }

    public string? Address { get; set; }

    public bool? IsDeleted { get; set; }

    public int? Otp { get; set; }

    public string? FacebookId { get; set; }

    public virtual ICollection<FriendRequest> FriendRequestReceivers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<FriendRequest> FriendRequestSenders { get; set; } = new List<FriendRequest>();

    public virtual Role Role { get; set; } = null!;
}
