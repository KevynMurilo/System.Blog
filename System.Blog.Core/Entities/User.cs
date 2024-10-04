namespace System.Blog.Core.Entities;
public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public UserRole Role { get; set; } = UserRole.READER;
    public string? PhotoPath { get; set; }

    public ICollection<Post>? Posts { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public ICollection<Like>? Likes { get; set; }
 }

public enum UserRole
{
    READER = 0,
    WRITER = 1,
    ADMIN = 2,
}
