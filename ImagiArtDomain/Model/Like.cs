using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImagiArtDomain.Model;

public partial class Like : Entity
{
    //public int Id { get; set; }
    [Display(Name = "Юзер")]
    public int UserId { get; set; }
    [Display(Name = "Пост")]
    public int PostId { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
