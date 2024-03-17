using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImagiArtDomain.Model;

public partial class Comment : Entity
{
    //public int Id { get; set; }

    [Display(Name ="Автор")]
    public int UserId { get; set; }
    [Display(Name = "Пост")]
    public int PostId { get; set; }
    [Display(Name = "Коментар")]
    public string Caption { get; set; } = null!;

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
