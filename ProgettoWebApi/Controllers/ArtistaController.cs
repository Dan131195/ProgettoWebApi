using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgettoWebApi.DTOs.Artista;
using ProgettoWebApi.Models;
using ProgettoWebApi.Services;

namespace ProgettoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ArtistaController : ControllerBase
    {
        private readonly ArtistaService _service;
        private readonly ILogger<ArtistaController> _logger;

        public ArtistaController(ArtistaService service, ILogger<ArtistaController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var artisti = await _service.GetAllAsync();
            var result = artisti.Select(a => new ArtistaResponseDto
            {
                ArtistaId = a.ArtistaId,
                Nome = a.Nome,
                Genere = a.Genere,
                Biografia = a.Biografia
            });

            _logger.LogInformation("Artisti trovati: {Count}", result.Count());
            return Ok(new { message = "Artisti trovati", data = result });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artista = await _service.GetByIdAsync(id);
            if (artista == null)
            {
                _logger.LogWarning("Artista non trovato ID: {Id}", id);
                return NotFound(new { message = "Artista non trovato" });
            }

            return Ok(new ArtistaResponseDto
            {
                ArtistaId = artista.ArtistaId,
                Nome = artista.Nome,
                Genere = artista.Genere,
                Biografia = artista.Biografia
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArtistaRequestDto dto)
        {
            var artista = new Artista
            {
                Nome = dto.Nome,
                Genere = dto.Genere,
                Biografia = dto.Biografia
            };

            var created = await _service.CreateAsync(artista);
            _logger.LogInformation("Artista creato ID: {Id}", created.ArtistaId);

            return Ok(new { message = "Artista creato", data = created.ArtistaId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateArtistaRequestDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated ? Ok(new { message = "Artista aggiornato" }) : NotFound(new { message = "Artista non trovato" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok(new { message = "Artista eliminato" }) : NotFound(new { message = "Artista non trovato" });
        }
    }

}