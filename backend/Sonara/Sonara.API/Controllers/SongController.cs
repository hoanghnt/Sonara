using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.Application.DTOs.Song;
using Sonara.Application.Interfaces;

namespace Sonara.API.Controllers;

[ApiController]
[Route("api/songs")]
public class SongController : ControllerBase
{
    private readonly ISongService _songService;
    private readonly ISongStreamService _songStreamService;
    private readonly ILogger<SongController> _logger;

    public SongController(
        ISongService songService,
        ISongStreamService songStreamService,
        ILogger<SongController> logger)
    {
        _songService = songService;
        _songStreamService = songStreamService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] UploadSongDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _songService.UploadAsync(dto, userId);
            return Created("", "Upload Successfully!");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<SongResponseDto> songs = await _songService.GetAllAsync();
            return Ok(songs);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            SongResponseDto? song = await _songService.GetByIdAsync(id);
            if (song == null) return NotFound();
            return Ok(song);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}/stream")]
    public async Task<IActionResult> Stream(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _songStreamService.ResolveAsync(id, cancellationToken);
            if (result == null)
                return NotFound();

            if (!string.IsNullOrEmpty(result.RedirectUrl))
                return Redirect(result.RedirectUrl);

            if (string.IsNullOrEmpty(result.LocalPath) || !System.IO.File.Exists(result.LocalPath))
                return NotFound();

            return PhysicalFile(result.LocalPath, result.ContentType, enableRangeProcessing: true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery]string keyword)
    {
        try
        {
            List<SongResponseDto> songs = await _songService.SearchAsync(keyword);
            return Ok(songs);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _songService.Delete(id);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}