using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgettoWebApi.Services;
using ProgettoWebApi.DTOs.Biglietto;
using System.Security.Claims;

namespace ProgettoWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BigliettoController : ControllerBase
    {
        private readonly BigliettoService _bigliettoService;
        private readonly ILogger<BigliettoController> _logger;

        public BigliettoController(BigliettoService service, ILogger<BigliettoController> logger)
        {
            _bigliettoService = service;
            _logger = logger;
        }

        [HttpPost("acquista")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Acquista([FromBody] AcquistaBigliettoRequestDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Utente non autenticato");
                return Unauthorized(new { message = "Utente non autenticato" });
            }

            var biglietto = await _bigliettoService.AcquistaAsync(dto.EventoId, userId);

            if (biglietto == null)
            {
                _logger.LogWarning($"Nessun biglietto disponibile per EventoId: {dto.EventoId}");
                return BadRequest(new { message = "Nessun biglietto disponibile per questo evento" });
            }

            var response = new BigliettoResponseDto
            {
                BigliettoId = biglietto.BigliettoId,
                EventoId = biglietto.EventoId,
                EventoTitolo = biglietto.Evento?.Titolo,
                DataEvento = biglietto.Evento?.Data,
                ArtistaId = biglietto.ArtistaId,
                ArtistaNome = biglietto.Evento?.Artista?.Nome,
                UserId = biglietto.UserId,
                EmailUtente = biglietto.User?.Email,
                DataAcquisto = biglietto.DataAcquisto
            };

            _logger.LogInformation($"Biglietto acquistato - ID: {biglietto.BigliettoId}, UserId: {userId}");
            return Ok(new { message = "Acquisto completato", data = response });
        }

        [HttpGet("mytickets")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Tentativo di accesso non autenticato a /mytickets");
                return Unauthorized(new { message = "Utente non autenticato" });
            }

            var biglietti = await _bigliettoService.GetByUserIdAsync(userId);

            if (biglietti == null || !biglietti.Any())
            {
                _logger.LogInformation($"Nessun biglietto trovato per utente {userId}");
                return Ok(new { message = "Nessun biglietto trovato", data = new List<BigliettoResponseDto>() });
            }

            var dto = biglietti.Select(b => new BigliettoResponseDto
            {
                BigliettoId = b.BigliettoId,
                EventoId = b.EventoId,
                EventoTitolo = b.Evento?.Titolo,
                DataEvento = b.Evento?.Data,
                ArtistaId = b.ArtistaId,
                ArtistaNome = b.Evento?.Artista?.Nome,
                DataAcquisto = b.DataAcquisto,
                UserId = b.UserId,
                EmailUtente = b.User?.Email
            });

            _logger.LogInformation($"Restituiti {dto.Count()} biglietti per utente {userId}");

            return Ok(new { message = "Biglietti trovati", data = dto });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var biglietti = await _bigliettoService.GetAllAsync();

            var dto = biglietti.Select(b => new BigliettoResponseDto
            {
                BigliettoId = b.BigliettoId,
                EventoId = b.EventoId,
                EventoTitolo = b.Evento?.Titolo,
                DataEvento = b.Evento?.Data,
                ArtistaId = b.ArtistaId,
                ArtistaNome = b.Evento?.Artista?.Nome,
                DataAcquisto = b.DataAcquisto,
                UserId = b.UserId,
                EmailUtente = b.User?.Email
            });

            return Ok(new { message = "Tutti i biglietti", data = dto });
        }

        [HttpGet("disponibili")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetDisponibili()
        {
            var biglietti = await _bigliettoService.GetDisponibiliAsync();

            var dto = biglietti.Select(b => new BigliettoResponseDto
            {
                BigliettoId = b.BigliettoId,
                EventoId = b.EventoId,
                EventoTitolo = b.Evento?.Titolo,
                DataEvento = b.Evento?.Data,
                ArtistaId = b.ArtistaId,
                ArtistaNome = b.Evento?.Artista?.Nome,
                DataAcquisto = b.DataAcquisto
            });

            return Ok(new { message = "Biglietti disponibili", data = dto });
        }

        [HttpGet("venduti")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVenduti()
        {
            var biglietti = await _bigliettoService.GetVendutiAsync();

            var dto = biglietti.Select(b => new BigliettoResponseDto
            {
                BigliettoId = b.BigliettoId,
                EventoId = b.EventoId,
                EventoTitolo = b.Evento?.Titolo,
                DataEvento = b.Evento?.Data,
                ArtistaId = b.ArtistaId,
                ArtistaNome = b.Evento?.Artista?.Nome,
                DataAcquisto = b.DataAcquisto,
                UserId = b.UserId,
                EmailUtente = b.User?.Email
            });

            return Ok(new { message = "Biglietti venduti", data = dto });
        }
    }
}
