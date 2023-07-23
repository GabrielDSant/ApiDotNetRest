using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiCrudDotNet.Models;

namespace ApiCrudDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return _context.Products;
        }

        [HttpGet("{id}")]
        public ActionResult<Product> Get(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            return product;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Product newProduct)
        {
            _context.Products.Add(newProduct);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product updatedProduct)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            product.nome = updatedProduct.nome;
            product.email = updatedProduct.email;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
