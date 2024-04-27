//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace _1.Models;

//public partial class Post
//{
//    //[Key]
//    public int PostId { get; set; }

//    public string UserId { get; set; } = null!;

//    public string Caption { get; set; } = null!;

//    public virtual User User { get; set; } = null!;
//}

using System;
using System.Collections.Generic;

namespace InkCanvas.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string UserId { get; set; }

    public string Caption { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; }
}
