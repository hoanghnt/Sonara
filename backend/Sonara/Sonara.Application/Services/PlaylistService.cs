using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sonara.Application.DTOs.Playlist;
using Sonara.Application.DTOs.Song;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;

namespace Sonara.Application.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IPlaylistRepository _playlistRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<PlaylistService> _logger;

    public PlaylistService(IPlaylistRepository playlistRepository, ILogger<PlaylistService> logger,
        IUserRepository userRepository)
    {
        _playlistRepository = playlistRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task AddSongAsync(Guid playlistId, Guid songId, Guid userId)
    {
        var playlist = await _playlistRepository.GetByIdAsync(playlistId);
        if (playlist == null) throw new Exception("Playlist not found!");
        if (playlist.UserId != userId) throw new Exception("Not your playlist!");

        //Check song existed
        var alreadyExists = playlist.PlaylistSongs.Any(ps => ps.SongId == songId);
        if (alreadyExists) throw new Exception("Song already in playlist");

        //Create new PlaylistSong
        var playlistSong = new PlaylistSong
        {
            PlaylistId = playlistId,
            SongId = songId,
            Order = playlist.PlaylistSongs.Count + 1,
            AddedAt = DateTime.UtcNow
        };

        playlist.PlaylistSongs.Add(playlistSong);
        await _playlistRepository.SaveChangesAsync();
    }

    public async Task<PlaylistResponseDto> CreateAsync(CreatePlaylistDto dto, Guid userId)
    {
        var playlist = new Playlist
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _playlistRepository.AddAsync(playlist);
        await _playlistRepository.SaveChangesAsync();

        return new PlaylistResponseDto(playlist.Id, playlist.Name, playlist.Description, playlist.CreatedAt, []);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var playlist = await _playlistRepository.GetByIdAsync(id);
        if (playlist == null) throw new Exception("Playlist not found!");
        if (playlist.UserId != userId) throw new Exception("Not your playlist!");

        _playlistRepository.DeleteAsync(playlist);
        await _playlistRepository.SaveChangesAsync();
    }

    public async Task<List<PlaylistResponseDto>> GetAllByUserIdAsync(Guid userId)
    {
        var playlists = await _playlistRepository.GetAllByUserIdAsync(userId);
        if (!playlists.Any())
            return [];

        return playlists.Select(playlist =>
        {
            var songs = playlist.PlaylistSongs
                .OrderBy(ps => ps.Order)
                .Select(ps => new SongResponseDto(
                    ps.Song.Id, ps.Song.Title, ps.Song.Artist, ps.Song.Album,
                    ps.Song.Duration, ps.Song.FileSize, ps.Song.CoverImagePath ?? "", ps.Song.CreatedAt
                )).ToList();

            return new PlaylistResponseDto(
                playlist.Id, playlist.Name, playlist.Description, playlist.CreatedAt, songs
            );
        }).ToList();
    }

    public async Task<PlaylistResponseDto?> GetByIdAsync(Guid id)
    {
        var playlist = await _playlistRepository.GetByIdAsync(id);
        if (playlist == null) throw new Exception("Playlist not found!");

        var songs = playlist.PlaylistSongs
            .OrderBy(ps => ps.Order)
            .Select(ps => new SongResponseDto(
                ps.Song.Id, ps.Song.Title, ps.Song.Artist, ps.Song.Album,
                ps.Song.Duration, ps.Song.FileSize, ps.Song.CoverImagePath ?? "", ps.Song.CreatedAt
            )).ToList();

        return new PlaylistResponseDto(playlist.Id, playlist.Name, playlist.Description, playlist.CreatedAt, songs);
    }

    public async Task RemoveSongAsync(Guid playlistId, Guid songId, Guid userId)
    {
        var playlist = await _playlistRepository.GetByIdAsync(playlistId);
        if (playlist == null) throw new Exception("Playlist not found!");
        if (playlist.UserId != userId) throw new Exception("Not your playlist!");

        var playlistSong = playlist.PlaylistSongs.FirstOrDefault(ps => ps.SongId == songId);
        if (playlistSong == null) throw new Exception("Song not in playlist!");
        playlist.PlaylistSongs.Remove(playlistSong);
        await _playlistRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, UpdatePlaylistDto dto, Guid userId)
    {
        var playlist = await _playlistRepository.GetByIdAsync(id);
        if (playlist == null) throw new Exception("Playlist not found!");
        if (playlist.UserId != userId) throw new Exception("Not your playlist!");

        playlist.Name = dto.Name;
        playlist.Description = dto.Description;

        await _playlistRepository.SaveChangesAsync();
    }
}