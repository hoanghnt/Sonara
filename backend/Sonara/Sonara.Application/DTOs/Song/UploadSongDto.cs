using Microsoft.AspNetCore.Http;

namespace Sonara.Application.DTOs.Song;

public record UploadSongDto(string Title, string Artist, string Album, IFormFile File, IFormFile? CoverImage);