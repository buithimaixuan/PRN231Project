using System;
using System.Collections.Generic;

namespace PostService.Models;

public partial class CategoryPost
{
    public int CategoryPostId { get; set; }

    public string CategoryPostName { get; set; } = null!;

    public string? CategoryPostDescription { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
