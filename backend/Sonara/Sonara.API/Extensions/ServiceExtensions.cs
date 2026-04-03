using Microsoft.Extensions.Configuration;
using Sonara.Application.Interfaces;
using Sonara.Application.Services;
using Sonara.Application.Services.AuthService;
using Sonara.Application.Services.SongService;
using Sonara.Infrastructure.Configuration;
using Sonara.Infrastructure.Repositories;
using Sonara.Infrastructure.Services;

namespace Sonara.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SupabaseStorageOptions>(
            configuration.GetSection(SupabaseStorageOptions.SectionName));

        services.AddHttpClient<SupabaseStorageClient>();

        var supabase = configuration.GetSection(SupabaseStorageOptions.SectionName)
            .Get<SupabaseStorageOptions>();
        if (supabase?.IsConfigured == true)
            services.AddScoped<IFileService, SupabaseFileService>();
        else
            services.AddScoped<IFileService, FileService>();

        services.AddScoped<ISongStreamService, SongStreamService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ISongRepository, SongRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISongService, SongService>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<IPlaylistService, PlaylistService>();
        return services;
    }
}