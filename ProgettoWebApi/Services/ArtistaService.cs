using System;
using Microsoft.EntityFrameworkCore;
using ProgettoWebApi.Data;
using ProgettoWebApi.DTOs;
using ProgettoWebApi.DTOs.Artista;
using ProgettoWebApi.Models;

namespace ProgettoWebApi.Services
{
    public class ArtistaService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ArtistaService> _logger;

        public ArtistaService(ApplicationDbContext context, ILogger<ArtistaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Artista>> GetAllAsync()
        {
            try
            {
                return await _context.Artisti.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore durante il recupero degli artisti");
                return new();
            }
        }

        public async Task<Artista?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Artisti.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore recuperando artista ID {Id}", id);
                return null;
            }
        }

        public async Task<Artista> CreateAsync(Artista artista)
        {
            try
            {
                _context.Artisti.Add(artista);
                await _context.SaveChangesAsync();
                return artista;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore nella creazione artista");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, CreateArtistaRequestDto dto)
        {
            try
            {
                var artista = await _context.Artisti.FindAsync(id);
                if (artista == null) return false;

                artista.Nome = dto.Nome;
                artista.Genere = dto.Genere;
                artista.Biografia = dto.Biografia;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore aggiornando artista ID {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var artista = await _context.Artisti.FindAsync(id);
                if (artista == null) return false;

                _context.Artisti.Remove(artista);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore eliminando artista ID {Id}", id);
                return false;
            }
        }
    }


}