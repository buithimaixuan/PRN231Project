using System;
using System.Collections.Generic;

namespace Client.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? AccountId { get; set; }

    public int? PostId { get; set; }

    public string? Content { get; set; }

    public int? Rate { get; set; }

    public DateOnly CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Post? Post { get; set; }
}
