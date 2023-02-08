using Microsoft.EntityFrameworkCore;

namespace DatabaseProvider.UnitTest.Web.Models;

public partial class DatabaseProviderDbContext : DbContext
{
    public DatabaseProviderDbContext() { }
    public DatabaseProviderDbContext(DbContextOptions<DatabaseProviderDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Kalemler" },
            new Category { Id = 2, Name = "Kitaplar" });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}