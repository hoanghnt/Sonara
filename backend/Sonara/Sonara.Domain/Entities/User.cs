namespace Sonara.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<Song> Songs { get; set; } = new List<Song>();
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}