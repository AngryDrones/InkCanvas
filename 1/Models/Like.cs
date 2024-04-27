using System;
using System.Collections.Generic;

namespace InkCanvas.Models;

public partial class Like
{
    public int LikeId { get; set; }

    public string UserId { get; set; }

    public int PostId { get; set; }

    public virtual Post Post { get; set; }

    public virtual User User { get; set; }
}
