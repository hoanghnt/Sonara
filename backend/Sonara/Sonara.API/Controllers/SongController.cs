using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sonara.Application.DTOs.Song;
using Sonara.Application.Interfaces;
using Sonara.Domain.Entities;

namespace Sonara.API.Controllers;

[ApiController]
[Route("api/songs")]
public class SongController : ControllerBase
{
    private readonly ISongService _songService;
    private readonly ILogger<SongController> _logger;

    public SongController(ISongService songService, ILogger<SongController> logger)
    {
        _songService = songService;
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
    public async Task<IActionResult> Stream(Guid id)
    {
        try
        {
            var filePath = await _songService.GetFilePathAsync(id);
            if (!System.IO.File.Exists(filePath)) return NotFound("File not found on disk");

            return PhysicalFile(
                filePath,
                "audio/mpeg", 
                enableRangeProcessing: true
            );
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