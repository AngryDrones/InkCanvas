﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace InkCanvas.Models;

//[Table("AspNetUsers")]
public partial class User : IdentityUser
{
    public int Age { get; set; }
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Follow> FollowUsers { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> FollowFollowers { get; set; } = new List<Follow>();
}