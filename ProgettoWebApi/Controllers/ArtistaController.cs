using Microsoft.AspNetCore.Mvc;
using ProgettoWebApi.DTOs.Artista;
using ProgettoWebApi.Models;
using ProgettoWebApi.Services;

namespace ProgettoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            _logger.LogInformation("Richiesta GET: tutti gli artisti");
            var artisti = await _service.GetAllAsync();

            var dto = artisti.Select(a => new ArtistaResponseDto
            {
                ArtistaId = a.ArtistaId,
                Nome = a.Nome,
                Genere = a.Genere,
                Biografia = a.Biografia
            });

            return Ok(new { message = "Artisti trovati", data = dto });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var artista = await _service.GetByIdAsync(id);
            if (artista == null) return NotFound();

            _logger.LogInformation("Artista trovato: {Id}", id);

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
            _logger.LogInformation("Creazione artista: {Nome}", dto.Nome);

            var artista = new Artista
            {
                Nome = dto.Nome,
                Genere = dto.Genere,
                Biografia = dto.Biografia
            };

            var result = await _service.CreateAsync(artista);
            return Ok(new { message = "Artista creato", data = result.ArtistaId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateArtistaRequestDto dto)
        {
            _logger.LogInformation("Aggiornamento artista ID: {Id}", id);
            var updated = await _service.UpdateAsync(id, dto);
            return updated ? Ok(new { message = "Artista aggiornato" }) : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogWarning("Eliminazione artista ID: {Id}", id);
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok(new { message = "Artista eliminato" }) : NotFound();
        }
    }
}