using Least.ExampleAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Least.ExampleAPI;

public class ExampleDbContext : DbContext
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }

    public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
           
        builder.Entity<User>().HasData(
            new User() { Id = 1, Name = "test" },
            new User() { Id = 2, Name = "not test" },
            new User() { Id = 3, Name = "gin"}
        );

        builder.Entity<Book>().HasData(
            new Book(){Id = 1, Name = "Fahrenheit 451", UserId = 3},
            new Book(){Id = 2, Name = "Ishmael", UserId = 3}
        );
    }
}