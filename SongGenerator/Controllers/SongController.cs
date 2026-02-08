using Microsoft.AspNetCore.Mvc;
using SongGenerator.Services;

namespace SongGenerator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SongController : ControllerBase
{
    private readonly SongService _songService;

    public SongController()
    {
        _songService = new SongService();
    }

    [HttpGet]
    public IActionResult GetSongs(
        [FromQuery] double avgLikes,
        [FromQuery] int page = 1,
        [FromQuery] long? seed = null,
        [FromQuery] string region = "en")
    {
        var data = _songService.GenerateBatch(page, 20, region, avgLikes, seed);
        return Ok(data);
    }

    [HttpGet("/api/media/cover")]
    public IActionResult GetCover(long seed, string title, string artist)
    {
        var imageData = ImageGenerator.GenerateCover(seed, title, artist);
        return File(imageData, "image/png");
    }

    [HttpGet("/api/media/audio")]
    public IActionResult GetAudio(long seed)
    {
        var audioData = AudioGenerator.GenerateSong(seed, 10.0);
        return File(audioData, "audio/wav", enableRangeProcessing: true);
    }
}
