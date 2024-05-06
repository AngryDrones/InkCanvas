using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace InkCanvas.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string UserId { get; set; }

    [DisplayName("Заголовок")]
    public string Caption { get; set; }

    [DisplayName("Опис")]
    public string Description { get; set; }

    public string ImageUrl { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; }

    [DisplayName("Зображення")]
    [NotMapped]
    public IFormFile ImageFile { get; set; }
}
