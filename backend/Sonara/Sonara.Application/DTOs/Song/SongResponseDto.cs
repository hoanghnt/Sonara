namespace Sonara.Application.DTOs.Song;

public record SongResponseDto(Guid Id, string Title, string Artist, string Album, int Duration, long FileSize, string CoverImagePath, DateTime CreatedAt);