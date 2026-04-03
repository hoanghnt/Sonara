namespace Sonara.Application.Interfaces;

/// <summary>Local disk or Supabase Storage; stored DB paths use prefix <c>sb:</c> for cloud objects.</summary>
public interface IFileService
{
    int GetAudioDuration(string tempFilePath);

    /// <summary>Moves or uploads <paramref name="tempFilePath"/> then deletes it; returns stored reference.</summary>
    Task<string> CommitStoredFileAsync(string tempFilePath, string folder);

    Task DeleteFileAsync(string storedReference);
}
