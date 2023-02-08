using DatabaseProvider.UnitTest.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace UnitTestRealWorld.Test
{
    public class DatabaseProviderTest
    {
        protected DbContextOptions<DatabaseProviderDbContext> _dbContextOptions { get; private set; }

        public void SetContextOptions(DbContextOptions<DatabaseProviderDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
            SeedData();
        }

        public void SeedData()
        {
            using (DatabaseProviderDbContext context = new DatabaseProviderDbContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Categories.AddRange(new Category[] { new Category() { Name = "Kalemler" }, new Category() { Name = "Defterler" } });
                context.SaveChanges();

                context.Products.AddRange(new Product[] { new Product() {CategoryId=1, Name="Kurşun kalem", Price=5, Stock=100, Color="mavi" },
                    new Product() { CategoryId = 2, Name = "Matamatik defteri", Price = 10, Stock = 100, Color = "siyah" } });
                context.SaveChanges();
            }
        }
    }
}