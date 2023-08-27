using ApiDbCache.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiDbCache.Db;

public class ApplicationDatabase : DbContext
{
    public ApplicationDatabase(DbContextOptions<ApplicationDatabase> options) : base(options)
    {
        
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Book> Books { get; set; }
    
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=demo;User Id=sa;Password=@Password");
    //     }
    //     base.OnConfiguring(optionsBuilder);
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Student>(m => m.HasMany<Book>(s => s.Books));
    }
}