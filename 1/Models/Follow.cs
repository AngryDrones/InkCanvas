using System;
using System.Collections.Generic;

namespace InkCanvas.Models;

public partial class Follow
{
    public int FollowId { get; set; }

    public string UserId { get; set; }

    public string FollowerId { get; set; }

    public virtual User Follower { get; set; }

    public virtual User User { get; set; }
}
