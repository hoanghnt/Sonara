using Sonara.Domain.Entities;

namespace Sonara.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    void Remove(RefreshToken token);
    Task SaveChangesAsync();
}