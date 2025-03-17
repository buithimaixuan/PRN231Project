using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PostService.Models;

public partial class LikePost
{
    public int LikePostId { get; set; }

    public int PostId { get; set; }

    public int AccountId { get; set; }

    public int? UnLike { get; set; }
    [JsonIgnore]
    public virtual Post Post { get; set; } = null!;
}
