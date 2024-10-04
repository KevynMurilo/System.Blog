namespace System.Blog.Core.Entities;

public class Tag
{
    public Guid TagId { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Post>? Posts { get; set; }
}
