using System;
using System.Collections.Generic;

namespace Client.Models;

public partial class View
{
    public int CountViewId { get; set; }

    public int AccountId { get; set; }

    public int? PostId { get; set; }

    public int? NewsId { get; set; }

    public virtual Post? Post { get; set; }
}
