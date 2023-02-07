using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestRealWorld.Web.Controllers;
using UnitTestRealWorld.Web.Models;
using UnitTestRealWorld.Web.Repositories;
using Xunit;

namespace UnitTestRealWorld.Test
{
    public class ProductApiControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsApiController _controller;
        private List<Product> _products;

        public ProductApiControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsApiController(_mockRepo.Object);
            _products = new List<Product>() {
                new Product { Id=1, Name = "Kalem", Price=100, Stock=100, Color="Kırmızı" },
                new Product { Id=2, Name = "Defter", Price=200, Stock=200, Color="Mavi" },
                new Product { Id=3, Name = "Kitap", Price=300, Stock=300, Color="Sarı" },
            };
        }

        [Fact]
        public async void GetProducts_ActionExecutes_ReturnOkResultProducts()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);
            var result = await _controller.GetProducts();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(3, returnProducts.Count());
            Assert.Equal(200, okResult.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void GetProduct_IdValid_ReturnOkResultProduct(int id)
        {
            Product product = _products.First(x => x.Id == id);
            _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = await _controller.GetProduct(id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(product.Id, returnProduct.Id);
            Assert.Equal(product.Name, returnProduct.Name);
            Assert.Equal(product.Price, returnProduct.Price);
            Assert.Equal(product.Stock, returnProduct.Stock);
            Assert.Equal(product.Color, returnProduct.Color);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        public async void GetProduct_IdInValid_ReturnNotFound(int id)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = await _controller.GetProduct(id);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        public void PutProduct_IdIsNotEqual_ReturnBadRequest(int id)
        {
            Product product = _products.First();
            var result = _controller.PutProduct(id, product);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public void PutProduct_ActionExecutes_ReturnNoContent(int id)
        {
            Product product = _products.First(x => x.Id == id);
            _mockRepo.Setup(x => x.Update(product));
            var result = _controller.PutProduct(id, product);
            _mockRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public async void PostProduct_ActionExecutes_ReternCreatedAtAction()
        {
            Product product = _products.First();
            _mockRepo.Setup(x => x.Create(product)).Returns(Task.CompletedTask);
            var result = await _controller.PostProduct(product);
            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetProduct", createdAtActionResult.ActionName);
            Assert.Equal(201, createdAtActionResult.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        public async void DeleteProduct_IdInValid_ReturnNotFound(int id)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = await _controller.DeleteProduct(id);
            var notFountResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFountResult.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteProduct_IdValid_ReturnNoContent(int id)
        {
            Product product = _products.First(x => x.Id == id);
            _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = await _controller.DeleteProduct(id);
            _mockRepo.Verify(x => x.Delete(It.IsAny<Product>()), Times.Once);
            var notFountResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, notFountResult.StatusCode);
        }

        [Theory]
        [InlineData(0)]
        public void ProductExists_IdInValid_ReturnFalse(int id)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = _controller.ProductExists(id);
            Assert.False(result);
        }

        [Theory]
        [InlineData(1)]
        public void ProductExists_IdValid_ReturnTrue(int id)
        {
            Product product = _products.First(x => x.Id == id);
            _mockRepo.Setup(x => x.GetById(id)).ReturnsAsync(product);
            var result = _controller.ProductExists(id);
            Assert.True(result);
        }
    }
}