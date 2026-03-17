using Sonara.Domain.Entities;

namespace Sonara.Application.Interfaces;

public interface IPlaylistRepository
{
    Task AddAsync(Playlist playlist);
    Task<Playlist?> GetByIdAsync(Guid id);
    Task<List<Playlist>> GetAllByUserIdAsync(Guid id);
    void DeleteAsync(Playlist playlist);
    Task SaveChangesAsync();
}