using Sonara.Application.DTOs.Song;

namespace Sonara.Application.DTOs.Playlist;

public record PlaylistResponseDto(Guid Id, string Name, string Description, DateTime CreatedAt, List<SongResponseDto> Songs);