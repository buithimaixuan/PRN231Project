using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PostService.Models;

public partial class CategoryNews
{
    public int CategoryNewsId { get; set; }

    public string CategoryNewsName { get; set; } = null!;

    public string? CategoryNewsDescription { get; set; }

    [JsonIgnore]
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
