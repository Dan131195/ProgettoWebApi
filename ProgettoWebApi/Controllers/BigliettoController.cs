using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ProgettoWebApi.DTOs.Biglietto;
using ProgettoWebApi.DTOs.Evento;
using ProgettoWebApi.Models;
using ProgettoWebApi.Services;

namespace ProgettoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BigliettoController : ControllerBase
    {
        private readonly BigliettoService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BigliettoController> _logger;

        public BigliettoController(BigliettoService service, IHttpContextAccessor accessor, ILogger<BigliettoController> logger)
        {
            _service = service;
            _httpContextAccessor = accessor;
            _logger = logger;
        }

        [HttpPost("acquista")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Acquista([FromBody] AcquistaBigliettoRequestDto dto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var biglietto = await _service.AcquistaAsync(dto.EventoId, userId);

            if (biglietto == null)
            {
                _logger.LogWarning("Biglietto non disponibile per EventoId {EventoId}", dto.EventoId);
                return BadRequest(new { message = "Biglietto non disponibile" });
            }

            return Ok(new { message = "Acquisto completato", data = biglietto });
        }

        [HttpGet("mytickets")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMiei()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var biglietti = await _service.GetByUserIdAsync(userId);

            var dto = biglietti.Select(b => new BigliettoResponseDto
            {
                BigliettoId = b.BigliettoId,
                EventoId = b.EventoId,
                TitoloEvento = b.Evento.Titolo,
                DataEvento = b.Evento.Data,
                ArtistaNome = b.Evento.Artista?.Nome,
                DataAcquisto = b.DataAcquisto
            });

            return Ok(new { message = "Biglietti trovati", data = dto });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTutti()
        {
            var biglietti = await _service.GetAllAsync();

            var dto = biglietti.Select(b => new BigliettoResponseDto
            {
                BigliettoId = b.BigliettoId,
                EventoId = b.EventoId,
                TitoloEvento = b.Evento.Titolo,
                DataEvento = b.Evento.Data,
                ArtistaNome = b.Evento.Artista?.Nome,
                DataAcquisto = b.DataAcquisto
            });

            return Ok(new { message = "Biglietti venduti", data = dto });
        }
    }

}