using Microsoft.AspNetCore.Mvc;
using UnitTestRealWorld.Web.Models;
using UnitTestRealWorld.Web.Repositories;

namespace UnitTestRealWorld.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IRepository<Product> _repository;

        public ProductsApiController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        // GET: api/ProductsApi
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _repository.GetAll();
            return Ok(products);
        }

        // GET: api/ProductsApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _repository.GetById(id);
            if (product == null) { return NotFound(); }
            return Ok(product);
        }

        // PUT: api/ProductsApi/5
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            if (id != product.Id) { return BadRequest(); }
            _repository.Update(product);
            return NoContent();
        }

        // POST: api/ProductsApi
        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _repository.Create(product);
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/ProductsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetById(id);
            if (product == null) { return NotFound(); }
            _repository.Delete(product);
            return NoContent();
        }

        [HttpGet("[action]")]
        public bool ProductExists(int id)
        {
            var product = _repository.GetById(id).Result;
            if (product == null)
                return false;
            else
                return true;
        }
    }
}