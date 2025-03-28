using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgettoWebApi.DTOs.Evento;
using ProgettoWebApi.Models;
using ProgettoWebApi.Services;

namespace ProgettoWebApi.DTOs.Biglietto
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _eventoService;
        private readonly BigliettoService _bigliettoService;
        private readonly ILogger<EventoController> _logger;

        public EventoController(EventoService service, BigliettoService bigliettoService, ILogger<EventoController> logger)
        {
            _eventoService = service;
            _bigliettoService = bigliettoService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventi = await _eventoService.GetAllAsync();

            var result = eventi.Select(e => new EventoResponseDto
            {
                EventoId = e.EventoId,
                Titolo = e.Titolo,
                Data = e.Data,
                Luogo = e.Luogo,
                ArtistaId = e.ArtistaId,
                ArtistaNome = e.Artista?.Nome
            });

            _logger.LogInformation("Eventi trovati: {Count}", result.Count());
            return Ok(new { message = "Eventi trovati", data = result });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evento = await _eventoService.GetByIdAsync(id);
            if (evento == null)
            {
                _logger.LogWarning("Evento non trovato ID: {Id}", id);
                return NotFound(new { message = "Evento non trovato" });
            }

            var dto = new EventoResponseDto
            {
                EventoId = evento.EventoId,
                Titolo = evento.Titolo,
                Data = evento.Data,
                Luogo = evento.Luogo,
                ArtistaId = evento.ArtistaId,
                ArtistaNome = evento.Artista?.Nome
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventoRequestDto dto)
        {
            _logger.LogInformation("🔧 Richiesta creazione evento ricevuta: {Titolo}", dto.Titolo);

            if (dto.QuantitaBiglietti <= 0)
            {
                _logger.LogWarning("❌ Quantità biglietti non valida: {Quantita}", dto.QuantitaBiglietti);
                return BadRequest(new { message = "Quantità biglietti deve essere maggiore di zero." });
            }

            var evento = new Models.Evento
            {
                Titolo = dto.Titolo,
                Data = dto.Data,
                Luogo = dto.Luogo,
                ArtistaId = dto.ArtistaId
            };

            // Salva evento
            var created = await _eventoService.CreateAsync(evento);

            if (created == null)
            {
                _logger.LogError("❌ Errore nella creazione dell'evento");
                return StatusCode(500, new { message = "Errore durante la creazione dell'evento." });
            }

            // Recupera evento completo con Artista per sicurezza
            var eventoSalvato = await _eventoService.GetByIdAsync(created.EventoId);

            if (eventoSalvato == null)
            {
                _logger.LogError("❌ Evento salvato non trovato. ID: {Id}", created.EventoId);
                return StatusCode(500, new { message = "Errore nel recupero evento dopo salvataggio." });
            }

            _logger.LogInformation("✅ Evento creato ID: {EventoId} - Creo {Quantita} biglietti...",
                eventoSalvato.EventoId, dto.QuantitaBiglietti);

            // Crea i biglietti associati
            await _bigliettoService.CreaBigliettiPerEvento(eventoSalvato, dto.QuantitaBiglietti);

            return Ok(new
            {
                message = "Evento creato con successo. Biglietti generati.",
                eventoId = eventoSalvato.EventoId
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEventoRequestDto dto)
        {
            var updated = await _eventoService.UpdateAsync(id, dto);
            return updated ? Ok(new { message = "Evento aggiornato" }) : NotFound(new { message = "Evento non trovato" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bigliettoService.DeleteByEventoIdAsync(id);
            var deleted = await _eventoService.DeleteAsync(id);
            return deleted ? Ok(new { message = "Evento e biglietti eliminati" }) : NotFound(new { message = "Evento non trovato" });
        }
    }

}