using Sonara.Application.Interfaces;

namespace Sonara.Infrastructure.Services;

public class FileService : IFileService
{
    public int GetAudioDuration(string tempFilePath)
    {
        using var file = TagLib.File.Create(tempFilePath);
        return (int)file.Properties.Duration.TotalSeconds;
    }

    public Task<string> CommitStoredFileAsync(string tempFilePath, string folder)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(tempFilePath);
        var folderPath = Path.Combine("upload", folder);
        Directory.CreateDirectory(folderPath);
        var destPath = Path.Combine(folderPath, fileName);

        File.Move(tempFilePath, destPath, overwrite: true);

        return Task.FromResult(destPath);
    }

    public Task DeleteFileAsync(string storedReference)
    {
        if (string.IsNullOrEmpty(storedReference))
            return Task.CompletedTask;

        if (storedReference.StartsWith(SupabaseFileService.StorageKeyPrefix, StringComparison.Ordinal))
            return Task.CompletedTask;

        var path = Path.GetFullPath(storedReference);
        if (File.Exists(path))
            File.Delete(path);

        return Task.CompletedTask;
    }
}
