using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiCrudDotNet.Models;

namespace ApiCrudDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private static List<Item> _items = new List<Item>();
        private static int _nextId = 1;

        [HttpGet]
        public ActionResult<IEnumerable<Item>> Get()
        {
            return _items;
        }

        [HttpGet("{id}")]
        public ActionResult<Item> Get(int id)
        {
            var item = _items.Find(i => i.Id == id);
            if (item == null)
                return NotFound();

            return item;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Item newItem)
        {
            newItem.Id = _nextId++;
            _items.Add(newItem);
            return CreatedAtAction(nameof(Get), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Item updatedItem)
        {
            var item = _items.Find(i => i.Id == id);
            if (item == null)
                return NotFound();

            item.Name = updatedItem.Name;
            item.Price = updatedItem.Price;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var item = _items.Find(i => i.Id == id);
            if (item == null)
                return NotFound();

            _items.Remove(item);
            return NoContent();
        }
    }
}
