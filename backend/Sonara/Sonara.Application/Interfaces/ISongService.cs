using Sonara.Application.DTOs.Song;

namespace Sonara.Application.Interfaces;

public interface ISongService
{
    Task<SongResponseDto> UploadAsync(UploadSongDto dto, Guid userId);
    Task<SongResponseDto?> GetByIdAsync(Guid id);
    Task<List<SongResponseDto>> GetAllAsync();
    Task<string?> GetFilePathAsync(Guid id);
    Task<List<SongResponseDto>> SearchAsync(string keyword);
    Task Delete(Guid id);
}