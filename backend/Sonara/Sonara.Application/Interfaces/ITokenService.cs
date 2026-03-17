using Sonara.Domain.Entities;

namespace Sonara.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}