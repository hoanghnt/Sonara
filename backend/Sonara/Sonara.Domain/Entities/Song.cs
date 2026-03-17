namespace Sonara.Domain.Entities;

public class Song
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? CoverImagePath { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; } = null!;
    public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}