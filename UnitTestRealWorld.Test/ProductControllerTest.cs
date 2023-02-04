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
        public async void Index_ActionExecutes_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()
        {
            _mockRepo.Setup(x => x.GetAll()).ReturnsAsync(_products);
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
            Assert.Equal(3, productList.Count());
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_IdValid_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Details(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
            Assert.Equal(product.Price, resultProduct.Price);
            Assert.Equal(product.Stock, resultProduct.Stock);
        }

        [Fact]
        public void Create_ActionExecutes_ReturnView()
        {
            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePOST_InValid_ModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name Alanı gereklidir");
            var result = await _controller.Create(_products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePOST_Valid_ModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(_products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePOST_Valid_ModelState_CreateMethodExecute()
        {
            Product product = null;
            _mockRepo.Setup(x => x.Create(It.IsAny<Product>())).Callback<Product>(x => product = x);
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Once);
            Assert.Equal(_products.First().Id, product.Id);
        }

        [Fact]
        public async void CreatePOST_InValid_ModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", " ");
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(x => x.Create(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(5)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            Product product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
            Assert.Equal(product.Stock, resultProduct.Stock);
            Assert.Equal(product.Price, resultProduct.Price);
            Assert.Equal(product.Color, resultProduct.Color);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, _products.First(x => x.Id == productId));
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = _controller.Edit(productId, _products.First(x => x.Id == productId));
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _controller.Edit(productId, _products.First(x => x.Id == productId));
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            Product product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.Update(product));
            _controller.Edit(productId, product);
            _mockRepo.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }
    }
}