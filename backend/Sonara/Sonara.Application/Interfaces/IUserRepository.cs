using Sonara.Domain.Entities;

namespace Sonara.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}

