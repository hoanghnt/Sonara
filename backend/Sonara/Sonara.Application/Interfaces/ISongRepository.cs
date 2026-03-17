using Sonara.Domain.Entities;

namespace Sonara.Application.Interfaces;

public interface ISongRepository
{
    Task AddAsync(Song song);
    Task<Song?> GetByIdAsync(Guid id);
    Task<List<Song>> GetAllAsync();
    Task SaveChangesAsync();
    Task<List<Song>> SearchAsync(string keyword);
    void Delete(Song song);
}