namespace Sonara.Application.Interfaces;

public sealed record SongStreamResult(
    string? LocalPath,
    string? RedirectUrl,
    string ContentType);

public interface ISongStreamService
{
    Task<SongStreamResult?> ResolveAsync(Guid songId, CancellationToken cancellationToken = default);
}
