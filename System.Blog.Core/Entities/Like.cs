namespace System.Blog.Core.Entities;

public class Like
{
    public Guid LikeId { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid PostId { get; set; }
    public Post? Post { get; set; }

    public DateTime? DateLike { get; set; } = DateTime.UtcNow;
}
