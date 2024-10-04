namespace System.Blog.Core.Entities;

public class Category
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhotoPath { get; set; } = string.Empty;

    public ICollection<Post>? Posts { get; set; }
}
