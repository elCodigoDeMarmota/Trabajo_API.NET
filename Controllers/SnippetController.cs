using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trabajo_API.NET.Models;


namespace Trabajo_API_NET.Controllers
{
    [ApiController]
    [Route("api/snippets")]   // 👈 ruta fija, REST, sin tokens raros
    public class SnippetController : ControllerBase
    {
        // POST /api/snippets
        [HttpPost]
        public IActionResult Create([FromBody] SnippetCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("The codigo es requerido.");

            var id = "demo123"; // mock temporal
            return Created($"/api/snippets/{id}", new { id });
        }

        // GET /api/snippets/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            if (id != "demo123")
                return NotFound();

            var demo = new SnippetResponse
            {
                Id = "demo123",
                Code = "Console.WriteLine(\"Hello\");",
                Language = "csharp",
                CreatedAt = DateTime.UtcNow
            };
            return Ok(demo);
        }
    }
}

