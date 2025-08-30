using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trabajo_API_NET.Data;
using Trabajo_API_NET.Models;
using Trabajo_API_NET.Servicios;

namespace Trabajo_API_NET.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // -> api/snippets
    public class SnippetController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IGeneradorID _generadorID;
        private readonly BuscarSnippet _service;



        public SnippetController(AppDbContext db, IGeneradorID generadorID, BuscarSnippet service)
        {
            _db = db;
            _generadorID = generadorID;
            _service = service;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SnippetCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("The 'code' field is required.");

            string id;
            do { id = _generadorID.NuevoID(7); }
            while (await _db.Snippets.AnyAsync(s => s.Id == id));

            var entity = new Snippet
            {
                Id = id,
                Code = request.Code,
                Language = request.Language,
                CreatedAt = DateTime.UtcNow
            };

            _db.Snippets.Add(entity);
            await _db.SaveChangesAsync();

            return Created($"/api/snippets/{id}", new { id });
        }

        [HttpGet("{id}/code")]
        public async Task<IActionResult> GetCodeById(string id)
        {
            var code = await _service.GetCodeByIdAsync(id);
            if (code is null) return NotFound(new { message = "Snippet no encontrado" });
            return Ok(new { Code = code });
        }




    }
}
