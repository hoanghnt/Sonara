using Microsoft.AspNetCore.Http;

namespace Sonara.Application.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, string folder);
    void DeleteFile(string filePath);
    int GetAudioDuration(string filePath);
}