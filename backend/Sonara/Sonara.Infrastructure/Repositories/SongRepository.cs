using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;
using Sonara.Infrastructure.Data;

namespace Sonara.Infrastructure.Repositories;

public class SongRepository : ISongRepository
{
    private readonly SonaraDbContext _context;
    private readonly ILogger<SongRepository> _logger;

    public SongRepository(SonaraDbContext context, ILogger<SongRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Song song)
    {
        await _context.Songs.AddAsync(song);
    }

    public void Delete(Song song)
    {
        _context.Songs.Remove(song);
    }

    public async Task<List<Song>> GetAllAsync()
    {
        return await _context.Songs.ToListAsync();
    }

    public async Task<Song?> GetByIdAsync(Guid id)
    {
        return await _context.Songs.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Song>> SearchAsync(string keyword)
    {
       return await _context.Songs.Where(s => s.Title.Contains(keyword) || s.Artist.Contains(keyword)).ToListAsync();
    }
}