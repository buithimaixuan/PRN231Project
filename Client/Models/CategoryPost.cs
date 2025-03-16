using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Client.Models;

public partial class CategoryPost
{
    public int CategoryPostId { get; set; }

    public string CategoryPostName { get; set; } = null!;

    public string? CategoryPostDescription { get; set; }
    [JsonIgnore]

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
