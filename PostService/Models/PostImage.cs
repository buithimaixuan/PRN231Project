using System;
using System.Collections.Generic;

namespace PostService.Models;

public partial class PostImage
{
    public int PostImageId { get; set; }

    public int PostId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsDeleted { get; set; }
    public string PublicId { get; set; }

    public virtual Post Post { get; set; } = null!;
}
