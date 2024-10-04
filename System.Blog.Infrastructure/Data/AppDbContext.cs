using Microsoft.EntityFrameworkCore;
using System.Blog.Core.Entities;

namespace System.Blog.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Like { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }
}
