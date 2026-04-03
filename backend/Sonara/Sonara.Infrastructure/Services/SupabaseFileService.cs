using Microsoft.Extensions.Options;
using Sonara.Application.Interfaces;
using Sonara.Infrastructure.Configuration;

namespace Sonara.Infrastructure.Services;

public sealed class SupabaseFileService : IFileService
{
    public const string StorageKeyPrefix = "sb:";

    private readonly SupabaseStorageClient _client;
    private readonly SupabaseStorageOptions _options;

    public SupabaseFileService(SupabaseStorageClient client, IOptions<SupabaseStorageOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public int GetAudioDuration(string tempFilePath)
    {
        using var file = TagLib.File.Create(tempFilePath);
        return (int)file.Properties.Duration.TotalSeconds;
    }

    public async Task<string> CommitStoredFileAsync(string tempFilePath, string folder)
    {
        if (!_options.IsConfigured)
            throw new InvalidOperationException("Supabase storage is not configured.");

        var ext = Path.GetExtension(tempFilePath);
        var objectKey = $"{folder.Trim('/')}/{Guid.NewGuid():N}{ext}".Replace('\\', '/');
        var contentType = GuessContentType(ext);

        await using (var stream = File.OpenRead(tempFilePath))
        {
            await _client.UploadObjectAsync(objectKey, stream, contentType);
        }

        try
        {
            File.Delete(tempFilePath);
        }
        catch
        {
            // ignore
        }

        return $"{StorageKeyPrefix}{objectKey}";
    }

    public async Task DeleteFileAsync(string storedReference)
    {
        if (string.IsNullOrEmpty(storedReference))
            return;

        if (!storedReference.StartsWith(StorageKeyPrefix, StringComparison.Ordinal))
            return;

        var key = storedReference[StorageKeyPrefix.Length..].TrimStart('/');
        await _client.DeleteObjectAsync(key);
    }

    private static string GuessContentType(string ext) =>
        ext.ToLowerInvariant() switch
        {
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".flac" => "audio/flac",
            ".m4a" => "audio/mp4",
            ".ogg" => "audio/ogg",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
}
