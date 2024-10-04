namespace System.Blog.Core.Entities;

public class Comment
{
    public Guid CommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime DateComment { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid PostId { get; set; }
    public Post? Post { get; set; }

    //Self-referral relationship
    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment>? Replies { get; set; }

}
