using Sonara.Application.DTOs.Auth;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;

namespace Sonara.Application.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null)
            throw new Exception("User not found!");
        var result = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!result)
            throw new Exception("Wrong Password!");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();
        return new AuthResponseDto(accessToken, refreshToken);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(dto.RefreshToken);
        if (tokenEntity == null) throw new Exception("Invalid refresh token!");

        if (tokenEntity.ExpiresAt < DateTime.UtcNow) throw new Exception("Refresh token expired!");

        var user = await _userRepository.GetByIdAsync(tokenEntity.UserId);

        _refreshTokenRepository.Remove(tokenEntity);

        var accessToken = _tokenService.GenerateAccessToken(user!);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshToken,
            UserId = user!.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();
        return new AuthResponseDto(accessToken, newRefreshToken);
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existedEmail = await _userRepository.ExistsByEmailAsync(dto.Email);
        if (existedEmail)
        {
            throw new Exception("Email existed!");
        }

        var existedUsername = await _userRepository.ExistsByUsernameAsync(dto.Username);
        if (existedUsername)
        {
            throw new Exception("Username existed!");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        User user = new User();
        user.Email = dto.Email;
        user.Username = dto.Username;
        user.PasswordHash = passwordHash;
        user.CreatedAt = DateTime.UtcNow;

        await _userRepository.AddAsync(user);

        string accessToken = _tokenService.GenerateAccessToken(user);
        string refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshToken);
    }
}

