using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.Application.DTOs.Playlist;
using Sonara.Application.Interfaces;

namespace Sonara.API.Controllers;

[ApiController]
[Route("api/playlists")]
public class PlaylistController : ControllerBase
{
    private readonly IPlaylistService _playlistService;
    private readonly ILogger<PlaylistController> _logger;

    public PlaylistController(IPlaylistService playlistService, ILogger<PlaylistController> logger)
    {
        _playlistService = playlistService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllPlaylist()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            List<PlaylistResponseDto> playlists = await _playlistService.GetAllByUserIdAsync(userId);
            return Ok(playlists);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPlaylistById(Guid id)
    {
        try
        {
            PlaylistResponseDto? playlist = await _playlistService.GetByIdAsync(id);
            return Ok(playlist);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _playlistService.CreateAsync(dto, userId);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePlaylist([FromBody] UpdatePlaylistDto dto, Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _playlistService.UpdateAsync(id, dto, userId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePlaylist(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _playlistService.DeleteAsync(id, userId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{id}/songs/{songId}")]
    [Authorize]
    public async Task<IActionResult> AddPlaylistSong(Guid id, Guid songId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _playlistService.AddSongAsync(id, songId, userId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}/songs/{songId}")]
    [Authorize]
    public async Task<IActionResult> DeletePlaylistSong(Guid id, Guid songId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _playlistService.RemoveSongAsync(id, songId, userId);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}