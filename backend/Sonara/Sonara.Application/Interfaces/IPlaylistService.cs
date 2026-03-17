using Sonara.Application.DTOs.Playlist;

namespace Sonara.Application.Interfaces;

public interface IPlaylistService
{
    Task<PlaylistResponseDto> CreateAsync(CreatePlaylistDto dto, Guid userId);
    Task<List<PlaylistResponseDto>> GetAllByUserIdAsync(Guid userId);
    Task<PlaylistResponseDto?> GetByIdAsync(Guid id);
    Task UpdateAsync(Guid id, UpdatePlaylistDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task AddSongAsync(Guid playlistId, Guid songId, Guid userId);
    Task RemoveSongAsync(Guid playlistId, Guid songId, Guid userId);
}