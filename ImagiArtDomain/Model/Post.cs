using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImagiArtDomain.Model;

public partial class Post : Entity
{
    //public int Id { get; set; }
    [Display(Name = "Автор")]
    public int UserId { get; set; }

    [Display(Name = "Заголовок")]
    public string Caption { get; set; } = null!;

    [Display(Name = "Опис")]
    public string? Description { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; } = null!;
}
