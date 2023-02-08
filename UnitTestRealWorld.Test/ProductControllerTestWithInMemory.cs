using DatabaseProvider.UnitTest.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace UnitTestRealWorld.Test
{
    public class ProductControllerTestWithInMemory : DatabaseProviderTest
    {
        public ProductControllerTestWithInMemory()
        {
            SetContextOptions(new DbContextOptionsBuilder<DatabaseProviderDbContext>().UseInMemoryDatabase("UnitTestInMemoryDB").Options);
        }

        [Fact]
        public async Task Create_ModelIsValid_ReturnRedirectToAction()
        {
            var newProduct = new Product { Name = "Test", Price=1, Stock=1, Color="test" };
            using (var context = new DatabaseProviderDbContext(_dbContextOptions))
            {
                var category = context.Categories.First();
                newProduct.CategoryId = category.Id;

                var controller = new DatabaseProvider.UnitTest.Web.Controllers.ProductsController(context);
                var result = await controller.Create(newProduct);
                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);
            }
            using (var context = new DatabaseProviderDbContext(_dbContextOptions))
            {
                var product = context.Products.FirstOrDefault(x => x.Name == newProduct.Name);
                Assert.Equal(newProduct.Name, product.Name);
            }
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteCategory_ExistCategoryId_DeletedAllProducts(int id)
        {
            using (var context = new DatabaseProviderDbContext(_dbContextOptions))
            {
                var category = await context.Categories.FindAsync(id);
                context.Categories.Remove(category);
                context.SaveChanges();
            }
            using (var context = new DatabaseProviderDbContext(_dbContextOptions))
            {
                var products = await context.Products.Where(x => x.CategoryId == id).ToListAsync();
                Assert.Empty(products);
            }
        }
    }
}