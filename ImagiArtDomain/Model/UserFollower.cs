using System;
using System.Collections.Generic;

namespace ImagiArtDomain.Model;

public partial class UserFollower : Entity
{
    //public int Id { get; set; }

    public int UserId { get; set; }

    public int FollowerId { get; set; }

    public virtual User User { get; set; } = null!;
}
