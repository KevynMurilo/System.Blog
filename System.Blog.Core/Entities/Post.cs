namespace System.Blog.Core.Entities;

public class Post
{
    public Guid PostId { get; set; }
    public string Title { get; set; } = string.Empty;  
    public string Slug { get; set; } = string.Empty;
    public string Content {  get; set; } = string.Empty;
    public DateTime PublicationDate { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; set; }
    public User? Author { get; set; }
    
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public List<string> Pictures { get; set; } = new List<string>();
    public ICollection<Tag>? Tags { get; set; }
    public ICollection<Comment>? Comments { get; set; }
}
