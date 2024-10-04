using System.Blog.Core.Entities;

namespace System.Blog.Core.Responses;

public class UserResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? PhotoPath { get; set; }
}
