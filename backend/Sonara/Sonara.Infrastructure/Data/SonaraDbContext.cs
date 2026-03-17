using Microsoft.EntityFrameworkCore;
using Sonara.Domain.Entities;

namespace Sonara.Infrastructure.Data;

public class SonaraDbContext : DbContext
{
    public SonaraDbContext(DbContextOptions<SonaraDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistSong> PlaylistSongs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlaylistSong>()
            .HasKey(ps => new { ps.PlaylistId, ps.SongId });

        modelBuilder.Entity<Song>()
            .HasOne(s => s.User)
            .WithMany(u => u.Songs)
            .HasForeignKey(s => s.UploadedBy);
        
        modelBuilder.Entity<Playlist>()
            .HasOne(p => p.User)
            .WithMany(u => u.Playlists)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<PlaylistSong>()
            .HasOne(ps => ps.Playlist)
            .WithMany(p => p.PlaylistSongs)
            .HasForeignKey(p => p.PlaylistId);

        modelBuilder.Entity<PlaylistSong>()
            .HasOne(ps => ps.Song)
            .WithMany(s => s.PlaylistSongs)
            .HasForeignKey(ps => ps.SongId);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);
    }
}