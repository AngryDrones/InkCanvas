using System;
using System.Collections.Generic;

namespace ImagiArtDomain.Model;

public partial class Like : Entity
{
    //public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
