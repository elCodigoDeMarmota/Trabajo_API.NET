using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Trabajo_API_NET.Models;
using Trabajo_API_NET.Data;
using Trabajo_API_NET.Servicios;
using Microsoft.EntityFrameworkCore;

[Route("[controller]")] 
public class SnippetController : Controller
{
    private readonly AppDbContext _db;
    private readonly IGeneradorID _generadorID;
    private readonly BuscarSnippet _service;
    private readonly IHttpClientFactory _httpFactory;

    public SnippetController(AppDbContext db, IGeneradorID generadorID, BuscarSnippet service, IHttpClientFactory httpFactory)
    {
        _db = db;
        _generadorID = generadorID;
        _service = service;
        _httpFactory = httpFactory;
    }


    [HttpPost("api")]                // /Snippet/api
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> CreateApi([FromBody] SnippetCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return BadRequest("The 'code' field is required.");

        string id;
        do { id = _generadorID.NuevoID(7); }
        while (await _db.Snippets.AnyAsync(s => s.Id == id));

        var entity = new Snippet { Id = id, Code = request.Code, Language = request.Language, CreatedAt = DateTime.UtcNow };
        _db.Snippets.Add(entity);
        await _db.SaveChangesAsync();

        return Created($"/api/snippets/{id}", new { id });
    }

    [HttpGet("api/{id}/code")]    
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> GetCodeByIdApi(string id)
    {
        var code = await _service.GetCodeByIdAsync(id);
        if (code is null) return NotFound(new { message = "Snippet no encontrado" });
        return Ok(new { Code = code });
    }

    [HttpGet("Crear")]        
    public IActionResult Crear()
    {
        return View(new SnippetCreateRequest());
    }

    [HttpGet("embellecer/{id}")]
    public async Task<IActionResult> embellecerAsync(string id, [FromServices] IHttpClientFactory httpFactory)
    {
        var snip = await _db.Snippets.FirstOrDefaultAsync(s => s.Id == id);
        if (snip == null) return NotFound("No existe el snippet.");

        string lenguaje = snip.Language.ToLower() switch
        {
            "c#" or "csharp" => "1",
            "html" => "2",
            "javascript" => "3",
            _ => "1"
        };
        var client = httpFactory.CreateClient("Highlighter");
        var httpRes = await client.PostAsJsonAsync("/api/highlighter/format", new
        {
            Codigo = snip.Code,
            Lenguaje = lenguaje
        });
        if (!httpRes.IsSuccessStatusCode)
            return StatusCode((int)httpRes.StatusCode, await httpRes.Content.ReadAsStringAsync());

        var html = await httpRes.Content.ReadAsStringAsync();
        return Content(html, "text/html");
    }

        [HttpPost("Crear")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Crear([Bind("Code,Language")] SnippetCreateRequest model, CancellationToken ct)
{
    if (string.IsNullOrWhiteSpace(model.Code))
        ModelState.AddModelError(nameof(model.Code), "El código es requerido.");
    if (string.IsNullOrWhiteSpace(model.Language))
        ModelState.AddModelError(nameof(model.Language), "El lenguaje es requerido.");
    if (!ModelState.IsValid) return View(model);

    string id;
    do { id = _generadorID.NuevoID(7); }
    while (await _db.Snippets.AsNoTracking().AnyAsync(s => s.Id == id, ct));

    _db.Snippets.Add(new Snippet {
        Id = id, Code = model.Code, Language = model.Language, CreatedAt = DateTime.UtcNow
    });
    var rows = await _db.SaveChangesAsync(ct);
    if (rows == 0)
    {
        ModelState.AddModelError(string.Empty, "No se pudo guardar en la base de datos.");
        return View(model);
    }

    // Pasa el ID al siguiente request
    TempData["SnippetId"] = id;

    // (Opcional) si quieres, elimina todo lo del Highlighter en este flujo
    // Aquí no devolvemos highlight porque vamos a redirigir
    return RedirectToAction(nameof(Crear));
}


    private static string MapLanguage(string language)
        => string.IsNullOrWhiteSpace(language) ? "plaintext" : language.Trim().ToLower() switch
        {
            "c#" or "cs" or "csharp" => "csharp",
            "js" or "javascript" => "javascript",
            "ts" or "typescript" => "typescript",
            "py" or "python" => "python",
            "html" => "html",
            "css" => "css",
            "sql" => "sql",
            "json" => "json",
            "xml" => "xml",
            _ => "plaintext"
        };
}
