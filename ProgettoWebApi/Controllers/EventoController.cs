using Microsoft.AspNetCore.Mvc;
using ProgettoWebApi.DTOs.Evento;
using ProgettoWebApi.Models;
using ProgettoWebApi.Services;

namespace ProgettoWebApi.DTOs.Biglietto
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly EventoService _service;
        private readonly BigliettoService _bigliettoService;
        private readonly ILogger<EventoController> _logger;

        public EventoController(EventoService service, BigliettoService bigliettoService, ILogger<EventoController> logger)
        {
            _service = service;
            _bigliettoService = bigliettoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var eventi = await _service.GetAllAsync();

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

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evento = await _service.GetByIdAsync(id);
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventoRequestDto dto)
        {
            var evento = new Models.Evento
            {
                Titolo = dto.Titolo,
                Data = dto.Data,
                Luogo = dto.Luogo,
                ArtistaId = dto.ArtistaId
            };

            var created = await _service.CreateAsync(evento);

            if (created == null)
            {
                _logger.LogError("Errore nella creazione evento");
                return BadRequest(new { message = "Errore nella creazione dell'evento" });
            }

            await _bigliettoService.CreaBigliettiPerEvento(created, dto.QuantitaBiglietti);

            _logger.LogInformation("Evento creato ID: {Id} con {Quantita} biglietti", created.EventoId, dto.QuantitaBiglietti);
            return Ok(new { message = "Evento creato con biglietti", data = created.EventoId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEventoRequestDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated ? Ok(new { message = "Evento aggiornato" }) : NotFound(new { message = "Evento non trovato" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bigliettoService.DeleteByEventoIdAsync(id);
            var deleted = await _service.DeleteAsync(id);
            return deleted ? Ok(new { message = "Evento e biglietti eliminati" }) : NotFound(new { message = "Evento non trovato" });
        }
    }

}