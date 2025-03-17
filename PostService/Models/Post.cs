using System;
using System.Collections.Generic;

namespace PostService.Models;

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

    public virtual CategoryPost CategoryPost { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<LikePost> LikePosts { get; set; } = new List<LikePost>();

    public virtual ICollection<SharePost> SharePosts { get; set; } = new List<SharePost>();

    public virtual ICollection<View> Views { get; set; } = new List<View>();
}
