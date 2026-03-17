using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;
using Sonara.Infrastructure.Data;

namespace Sonara.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SonaraDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(SonaraDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}