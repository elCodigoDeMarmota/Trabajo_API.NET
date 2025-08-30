using Microsoft.EntityFrameworkCore;
using Trabajo_API_NET.Data;
using Trabajo_API_NET.Models;

namespace Trabajo_API_NET.Servicios
{
    public class BuscarSnippet
    {
        private readonly AppDbContext _context;
        public BuscarSnippet(AppDbContext context) => _context = context;

        public async Task<string?> GetCodeByIdAsync(string id)
        {
            var filas = await _context.BuscarCodes
                .FromSqlInterpolated($"EXEC dbo.BuscarSnippet @Id={id}")
                .AsNoTracking()
                .ToListAsync();                 

            var fila = filas.FirstOrDefault();  
            return fila?.Code;
        }

    }
}
