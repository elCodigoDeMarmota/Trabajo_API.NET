using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trabajo_API_NET.Data;      
using Trabajo_API_NET.Models;    
using Trabajo_API_NET.Servicios;  

namespace Trabajo_API_NET.Controllers
{
    [ApiController]
    [Route("api/snippets")]
    public class SnippetController : ControllerBase
    {
        private readonly AppDbContext _db;        
        private readonly IGeneradorID _generadorID; 

        // Inyección de dependencias: DbContext + GeneradorID
        public SnippetController(AppDbContext db, IGeneradorID generadorID)
        {
            _db = db;
            _generadorID = generadorID;
        }

 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SnippetCreateRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("The 'code' field is required.");

            // Generar ID corto y evitar colisiones (muy poco probable)
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var entity = await _db.Snippets.FindAsync(id);
            if (entity is null) return NotFound();

            var response = new SnippetResponse
            {
                Id = entity.Id,
                Code = entity.Code,
                Language = entity.Language,
                CreatedAt = entity.CreatedAt
            };
            return Ok(response);
        }
    }
}


