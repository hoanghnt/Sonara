using Microsoft.Extensions.Options;
using Sonara.Application.Interfaces;
using Sonara.Infrastructure.Configuration;

namespace Sonara.Infrastructure.Services;

public sealed class SongStreamService : ISongStreamService
{
    private readonly ISongRepository _songRepository;
    private readonly SupabaseStorageClient _client;
    private readonly SupabaseStorageOptions _options;

    public SongStreamService(
        ISongRepository songRepository,
        SupabaseStorageClient client,
        IOptions<SupabaseStorageOptions> options)
    {
        _songRepository = songRepository;
        _client = client;
        _options = options.Value;
    }

    public async Task<SongStreamResult?> ResolveAsync(Guid songId, CancellationToken cancellationToken = default)
    {
        var song = await _songRepository.GetByIdAsync(songId);
        if (song == null)
            return null;

        var stored = song.FilePath;
        if (string.IsNullOrEmpty(stored))
            return null;

        if (stored.StartsWith(SupabaseFileService.StorageKeyPrefix, StringComparison.Ordinal))
        {
            if (!_options.IsConfigured)
                return null;

            var objectKey = stored[SupabaseFileService.StorageKeyPrefix.Length..].TrimStart('/');
            try
            {
                var signed = await _client.CreateSignedUrlAsync(objectKey, cancellationToken);
                var contentType = ContentTypeForPath(objectKey);
                return new SongStreamResult(null, signed, contentType);
            }
            catch
            {
                return null;
            }
        }

        var fullPath = Path.GetFullPath(stored);
        if (!File.Exists(fullPath))
            return null;

        return new SongStreamResult(fullPath, null, ContentTypeForPath(stored));
    }

    private static string ContentTypeForPath(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext switch
        {
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".flac" => "audio/flac",
            ".m4a" => "audio/mp4",
            ".ogg" => "audio/ogg",
            _ => "application/octet-stream"
        };
    }
}
