namespace InkCanvas.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string UserId { get; set; }

    public string Caption { get; set; }

    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; }
}
