using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;
using Sonara.Infrastructure.Data;

namespace Sonara.Infrastructure.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly SonaraDbContext _context;
    private readonly ILogger<PlaylistRepository> _logger;

    public PlaylistRepository(SonaraDbContext context, ILogger<PlaylistRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Playlist playlist)
    {
        await _context.Playlists.AddAsync(playlist);
    }

    public void DeleteAsync(Playlist playlist)
    {
        _context.Playlists.Remove(playlist);
    }

    public async Task<List<Playlist>> GetAllByUserIdAsync(Guid id)
    {
        return await _context.Playlists.Where(p => p.UserId == id).ToListAsync();
    }

    public async Task<Playlist?> GetByIdAsync(Guid id)
    {
        return await _context.Playlists.Include(p => p.PlaylistSongs).ThenInclude(ps => ps.Song)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}