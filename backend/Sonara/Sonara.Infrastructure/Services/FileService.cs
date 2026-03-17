using Microsoft.AspNetCore.Http;
using Sonara.Application.Interfaces;

namespace Sonara.Infrastructure.Services;

public class FileService : IFileService
{
    public void DeleteFile(string filePath)
    {
        if (filePath == null) throw new Exception("filePath not found!");
        File.Delete(filePath);

    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
    {
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

        var folderPath = Path.Combine("upload", folder);
        Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return filePath;
    }

    public int GetAudioDuration(string filePath)
    {
        var file = TagLib.File.Create(filePath);
        return (int)file.Properties.Duration.TotalSeconds;
    }
}