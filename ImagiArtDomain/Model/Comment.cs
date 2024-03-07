using System;
using System.Collections.Generic;

namespace ImagiArtDomain.Model;

public partial class Comment : Entity
{
    //public int Id { get; set; }

    public int UserId { get; set; }

    public int PostId { get; set; }

    public string Caption { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
