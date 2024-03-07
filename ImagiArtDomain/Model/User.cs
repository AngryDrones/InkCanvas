using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImagiArtDomain.Model;

public partial class User : Entity
{
    //public int Id { get; set; }
    [Display(Name = "Юзернейм")]
    public string Username { get; set; } = null!;

    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<UserFollower> UserFollowers { get; set; } = new List<UserFollower>();
}
