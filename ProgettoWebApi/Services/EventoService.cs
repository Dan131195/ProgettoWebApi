using Microsoft.EntityFrameworkCore;
using ProgettoWebApi.Data;
using ProgettoWebApi.DTOs.Evento;
using ProgettoWebApi.Models;
using System;

namespace ProgettoWebApi.Services
{
    public class EventoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventoService> _logger;

        public EventoService(ApplicationDbContext context, ILogger<EventoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Evento>> GetAllAsync()
        {
            try
            {
                return await _context.Eventi
                    .Include(e => e.Artista)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nel recupero eventi");
                return new();
            }
        }

        public async Task<Evento?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Eventi
                    .Include(e => e.Artista)
                    .FirstOrDefaultAsync(e => e.EventoId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore recuperando evento ID {Id}", id);
                return null;
            }
        }

        public async Task<Evento?> CreateAsync(Evento evento)
        {
            try
            {
                _context.Eventi.Add(evento);
                await _context.SaveChangesAsync();
                return evento;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione evento");
                return null;
            }
        }

        public async Task<bool> UpdateAsync(int id, CreateEventoRequestDto dto)
        {
            try
            {
                var evento = await _context.Eventi.FindAsync(id);
                if (evento == null) return false;

                evento.Titolo = dto.Titolo;
                evento.Data = dto.Data;
                evento.Luogo = dto.Luogo;
                evento.ArtistaId = dto.ArtistaId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore aggiornando evento ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var evento = await _context.Eventi.FindAsync(id);
                if (evento == null) return false;

                _context.Eventi.Remove(evento);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore eliminando evento ID {Id}", id);
                return false;
            }
        }
    }


}
