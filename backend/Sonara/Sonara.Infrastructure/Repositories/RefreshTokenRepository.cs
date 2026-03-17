using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;
using Sonara.Infrastructure.Data;

namespace Sonara.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SonaraDbContext _context;
    private readonly ILogger<RefreshToken> _logger;

    public RefreshTokenRepository(SonaraDbContext context, ILogger<RefreshToken> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
    }

    public void Remove(RefreshToken token)
    {
        _context.RefreshTokens.Remove(token);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}