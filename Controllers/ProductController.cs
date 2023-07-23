using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiCrudDotNet.Models;

namespace ApiCrudDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static List<Product> _products = new List<Product>();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return _products;
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            var product = _products.Find(i => i.Id == id);
            if (product == null)
                return NotFound();

            return product;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product newProduct)
        {
            newProduct.Id = _nextId++;
            _products.Add(newProduct);
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product updatedProduct)
        {
            var product = _products.Find(i => i.Id == id);
            if (product == null)
                return NotFound();

            product.nome = updatedProduct.nome;
            product.email = updatedProduct.email;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _products.Find(i => i.Id == id);
            if (product == null)
                return NotFound();

            _products.Remove(product);
            return NoContent();
        }
    }
}
