using AgvAppAuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AgvAppAuthService.UserDbContext;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed predefined users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "operator", PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"), Role = "Operator" },
            new User { Id = 2, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("AgvMasters2024"), Role = "Admin" }
        );
    }
}