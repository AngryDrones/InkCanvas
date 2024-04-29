using System;
using System.Collections.Generic;

namespace InkCanvas.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string UserId { get; set; }

    public int PostId { get; set; }

    public string Caption { get; set; }

    public DateTime Date { get; set; }

    public virtual Post Post { get; set; }

    public virtual User User { get; set; }
}
