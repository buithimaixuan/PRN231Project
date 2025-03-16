using System;
using System.Collections.Generic;

namespace Client.Models;

public partial class FriendRequest
{
    public int RequestId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public string RequestStatus { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public virtual Account Receiver { get; set; } = null!;

    public virtual Account Sender { get; set; } = null!;
}
