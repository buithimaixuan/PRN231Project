using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Client.Models;

public partial class Post
{
    public int PostId { get; set; }

    public int CategoryPostId { get; set; }

    public int? AccountId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public string? PostContent { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    [JsonIgnore]
    public virtual CategoryPost CategoryPost { get; set; } = null!;
    [JsonIgnore]

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]

    public virtual ICollection<LikePost> LikePosts { get; set; } = new List<LikePost>();
    [JsonIgnore]

    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
    [JsonIgnore]

    public virtual ICollection<SharePost> SharePosts { get; set; } = new List<SharePost>();
    [JsonIgnore]

    public virtual ICollection<View> Views { get; set; } = new List<View>();
}
