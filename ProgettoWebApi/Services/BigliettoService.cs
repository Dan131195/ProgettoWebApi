using Microsoft.EntityFrameworkCore;
using ProgettoWebApi.Data;
using ProgettoWebApi.DTOs.Biglietto;
using ProgettoWebApi.Models;
using System;

namespace ProgettoWebApi.Services
{
    public class BigliettoService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BigliettoService> _logger;

        public BigliettoService(ApplicationDbContext context, ILogger<BigliettoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Biglietto>> GetAllAsync()
        {
            try
            {
                return await _context.Biglietti
                    .Include(b => b.Evento)
                        .ThenInclude(e => e.Artista)
                    .Include(b => b.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero di tutti i biglietti");
                return new();
            }
        }

        public async Task<List<Biglietto>> GetDisponibiliAsync()
        {
            try
            {
                return await _context.Biglietti
                    .Include(b => b.Evento)
                        .ThenInclude(e => e.Artista)
                    .Where(b => b.UserId == null)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei biglietti disponibili");
                return new();
            }
        }

        public async Task<List<Biglietto>> GetVendutiAsync()
        {
            try
            {
                return await _context.Biglietti
                    .Include(b => b.Evento)
                        .ThenInclude(e => e.Artista)
                    .Include(b => b.User)
                    .Where(b => b.UserId != null)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei biglietti venduti");
                return new();
            }
        }

        public async Task<List<Biglietto>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Biglietti
                    .Include(b => b.Evento)
                    .ThenInclude(e => e.Artista)
                    .Include(b => b.User) 
                    .Where(b => b.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore biglietti per utente {UserId}", userId);
                return new();
            }
        }

        public async Task<Biglietto?> AcquistaAsync(int eventoId, string userId)
        {
            try
            {
                var bigliettoDisponibile = await _context.Biglietti
                    .Include(b => b.Evento)
                        .ThenInclude(e => e.Artista)
                    .Include(b => b.User)
                    .Where(b => b.EventoId == eventoId && b.UserId == null)
                    .FirstOrDefaultAsync();

                if (bigliettoDisponibile == null) return null;

                bigliettoDisponibile.UserId = userId;
                bigliettoDisponibile.DataAcquisto = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _context.Entry(bigliettoDisponibile).Reference(b => b.User).LoadAsync();

                return bigliettoDisponibile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Errore acquisto biglietto evento {eventoId}");
                return null;
            }
        }

        public async Task<bool> DeleteByEventoIdAsync(int eventoId)
        {
            try
            {
                var biglietti = await _context.Biglietti
                    .Where(b => b.EventoId == eventoId)
                    .ToListAsync();

                if (!biglietti.Any()) return false;

                _context.Biglietti.RemoveRange(biglietti);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Errore eliminazione biglietti per evento {eventoId}");
                return false;
            }
        }

        public async Task CreaBigliettiPerEvento(Evento evento, int quantita)
        {
            try
            {
                _logger.LogInformation($"Entrato in CreaBigliettiPerEvento con quantita: {quantita}");

                if (quantita <= 0)
                {
                    _logger.LogWarning($"Quantità biglietti non valida: {quantita}");
                    return;
                }

                var biglietti = Enumerable.Range(0, quantita).Select(b => new Biglietto
                {
                    EventoId = evento.EventoId,
                    ArtistaId = evento.ArtistaId,
                    DataAcquisto = DateTime.UtcNow,
                    UserId = null
                }).ToList();

                await _context.Biglietti.AddRangeAsync(biglietti);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Creati {biglietti.Count} biglietti per evento ID: {evento.EventoId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante la creazione dei biglietti (EventoId: {evento.EventoId})");
                throw; // solo per debug, poi puoi rimuoverlo
            }
        }
    }

}



