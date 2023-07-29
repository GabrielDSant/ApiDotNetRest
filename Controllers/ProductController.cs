using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ApiCrudDotNet.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ApiCrudDotNet.Controllers
{
    public class LogActionFilter : IActionFilter
    {
        private readonly AppDbContext _context;
        public LogActionFilter(AppDbContext Dbcontext)
        {
            _context = Dbcontext;
        }
        public void OnActionExecuted(ActionExecutedContext context, HttpContext httpContext)
        {
            // Executado após a ação do controlador ser executada.
            // Obter a URL da solicitação.
            string url = context.HttpContext.Request.Path;
            // Obter o corpo da solicitação como sequência (string).
            string requestBody;
            var request = context.HttpContext.Request;
            request.EnableBuffering(); // Habilitar bufferização para que o corpo possa ser lido novamente.
            request.Body.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = reader.ReadToEnd();
            }
            request.Body.Seek(0, SeekOrigin.Begin); // Voltar ao início do fluxo do corpo para que a solicitação ainda funcione corretamente.

            // Faça o que quiser com o corpo da solicitação, como salvar em um arquivo de log, enviar para um serviço, etc.
            // Por exemplo, você pode registrar a string 'requestBody' em um arquivo de log.

            // ...

            // Obter o objeto de resposta HTTP.
            HttpResponse response = context.HttpContext.Response;

            // Obter o status code da resposta.
            int statusCode = response.StatusCode;

            // Obter o corpo da resposta como sequência (string).
            string responseBody;
            response.Body.Seek(0, SeekOrigin.Begin); // Certifique-se de voltar ao início do fluxo do corpo.
            using (StreamReader reader = new StreamReader(response.Body, Encoding.UTF8))
            {
                responseBody = reader.ReadToEnd();
            }

            // Obter o IP de quem enviou a requisição.
            string requesterIp = context.HttpContext.Connection.RemoteIpAddress.ToString();


            // Faça o que quiser com o objeto de resposta e o corpo, como salvar em um arquivo de log, enviar para um serviço, etc.
            // Por exemplo, você pode registrar a string 'responseBody' em um arquivo de log.

            // ...
            Logger logging = new Logger
            {
                URL = url,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                RequesterIp = requesterIp,
                DT_HORA = DateTime.Now
            };

            _context.Logger.Add(newLogger);
            _context.SaveChanges();
            // Continuar com a execução normal da resposta.


        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }

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
