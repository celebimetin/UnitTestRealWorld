using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTestRealWorld.Web.Controllers;
using UnitTestRealWorld.Web.Models;
using UnitTestRealWorld.Web.Repositories;
using Xunit;

namespace UnitTestRealWorld.Test
{

    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            _products = new List<Product>() {
                new Product { Id=1, Name = "Kalem", Price=100, Stock=100, Color="Kırmızı" },
                new Product { Id=2, Name = "Defter", Price=200, Stock=200, Color="Mavi" },
                new Product { Id=3, Name = "Kitap", Price=300, Stock=300, Color="Sarı" },
            };
        }

        [Fact]
        //Geriye bir view dönmesi ile ilgili test
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        //Geriye bir Products List dönmesi ile ilgili test
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(_products);
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal<int>(3, productList.Count());
        }
    }
}