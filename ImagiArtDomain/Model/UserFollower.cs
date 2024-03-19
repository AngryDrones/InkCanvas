using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImagiArtDomain.Model;

public partial class UserFollower : Entity
{
    //public int Id { get; set; }
    [Display(Name = "Юзер")]
    public int UserId { get; set; }
    [Display(Name = "Підписник")]
    public int FollowerId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual User Follower { get; set; } = null!;
}
