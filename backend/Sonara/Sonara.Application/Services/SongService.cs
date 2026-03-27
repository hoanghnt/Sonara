using Microsoft.Extensions.Logging;
using Sonara.Application.DTOs.Song;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;

namespace Sonara.Application.Services.SongService;

public class SongService : ISongService
{
    private readonly ISongRepository _songRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<SongService> _logger;

    private const long MaxAudioBytes = 50L * 1024 * 1024;

    private const long MaxCoverBytes = 5L * 1024 * 1024;

    private static readonly HashSet<string> AllowedAudioExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".mp3", ".wav", ".flac", ".m4a", ".ogg" };
    
    private static readonly HashSet<string> AllowedCoverExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    public SongService(ISongRepository songRepository, IFileService fileService, ILogger<SongService> logger)
    {
        _songRepository = songRepository;
        _fileService = fileService;
        _logger = logger;
    }

    public async Task Delete(Guid id)
    {
        var song = await _songRepository.GetByIdAsync(id);
        if (song == null) throw new Exception("Song not found!");

        _songRepository.Delete(song);
        await _songRepository.SaveChangesAsync();
    }

    public async Task<List<SongResponseDto>> GetAllAsync()
    {
        var songs = await _songRepository.GetAllAsync();
        return songs.Select(song => new SongResponseDto(song.Id, song.Title, song.Artist, song.Album, song.Duration,
            song.FileSize, song.CoverImagePath ?? "", song.CreatedAt)).ToList();
    }

    public async Task<SongResponseDto?> GetByIdAsync(Guid id)
    {
        var song = await _songRepository.GetByIdAsync(id);
        if (song == null) throw new Exception("Song not found!");

        return new SongResponseDto(song.Id, song.Title, song.Artist, song.Album, song.Duration, song.FileSize,
            song.CoverImagePath ?? "", song.CreatedAt);
    }

    public async Task<string?> GetFilePathAsync(Guid id)
    {
        var song = await _songRepository.GetByIdAsync(id);
        if (song == null) throw new Exception("Song not found!");

        var filePath = Path.GetFullPath(song.FilePath);
        if (filePath == null) throw new Exception("File not found!");

        return filePath;
    }

    public async Task<List<SongResponseDto>> SearchAsync(string keyword)
    {
        var songs = await _songRepository.SearchAsync(keyword);
        return songs.Select(song => new SongResponseDto(song.Id, song.Title, song.Artist, song.Album, song.Duration,
            song.FileSize, song.CoverImagePath ?? "", song.CreatedAt)).ToList();
    }

    public async Task<SongResponseDto> UploadAsync(UploadSongDto dto, Guid userId)
    {
        ValidateUpload(dto);
        
        var filePath = await _fileService.SaveFileAsync(dto.File, "songs");
        var duration = _fileService.GetAudioDuration(filePath);
        string? coverImagePath = null;
        if (dto.CoverImage != null)
        {
            coverImagePath = await _fileService.SaveFileAsync(dto.CoverImage, "covers");
        }

        var song = new Song
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Artist = dto.Artist,
            Album = dto.Album,
            Duration = duration,
            FilePath = filePath,
            FileSize = dto.File.Length,
            CoverImagePath = coverImagePath,
            UploadedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _songRepository.AddAsync(song);
        await _songRepository.SaveChangesAsync();

        return new SongResponseDto(
            song.Id, song.Title, song.Artist, song.Album, song.Duration, song.FileSize, song.CoverImagePath ?? "",
            song.CreatedAt);
    }

    private static void ValidateUpload(UploadSongDto dto)
    {
        if (dto.File.Length <= 0)
            throw new ArgumentException("Audio file is empty.");
        if (dto.File.Length > MaxAudioBytes)
            throw new ArgumentException("Audio file must be 50 MB or smaller.");
        var audioExt = Path.GetExtension(dto.File.FileName);
        if (string.IsNullOrEmpty(audioExt) || !AllowedAudioExtensions.Contains(audioExt))
            throw new ArgumentException("Audio format not allowed. Use MP3, WAV, FLAC, M4A, or OGG.");
        if (dto.CoverImage is { Length: > 0 })
        {
            if (dto.CoverImage.Length > MaxCoverBytes)
                throw new ArgumentException("Cover image must be 5 MB or smaller.");
            var coverExt = Path.GetExtension(dto.CoverImage.FileName);
            if (string.IsNullOrEmpty(coverExt) || !AllowedCoverExtensions.Contains(coverExt))
                throw new ArgumentException("Cover must be JPG, PNG, WebP, or GIF.");
        }
    }
}