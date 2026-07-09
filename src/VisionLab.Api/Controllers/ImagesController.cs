using Microsoft.AspNetCore.Mvc;
using VisionLab.Application.Images;
using VisionLab.Core.Images;
using VisionLab.Shared.Images;

namespace VisionLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private const string GetImageByIdRouteName = "GetImageById";
    private readonly IImageAssetService _imageAssetService;

    public ImagesController(IImageAssetService imageAssetService)
    {
        _imageAssetService = imageAssetService;
    }

    [HttpPost]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<ImageAssetDto>> UploadAsync(
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null)
        {
            return BadRequest("File is required.");
        }

        if (file.Length <= 0)
        {
            return BadRequest("File is empty.");
        }

        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only image files are supported.");
        }

        await using var stream = file.OpenReadStream();

        var asset = await _imageAssetService.UploadAsync(
            stream,
            file.FileName,
            file.ContentType,
            file.Length,
            cancellationToken);

        return CreatedAtRoute(
            GetImageByIdRouteName,
            new { id = asset.Id.Value },
            MapToDto(asset));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ImageAssetDto>>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        var assets = await _imageAssetService.GetAllAsync(cancellationToken);

        var dtos = assets
            .Select(MapToDto)
            .ToArray();

        return Ok(dtos);
    }

    [HttpGet("{id:guid}", Name = GetImageByIdRouteName)]
    public async Task<ActionResult<ImageAssetDto>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var asset = await _imageAssetService.GetByIdAsync(
            new ImageAssetId(id),
            cancellationToken);

        if (asset is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(asset));
    }

    [HttpGet("{id:guid}/content")]
    public async Task<IActionResult> GetContentAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var content = await _imageAssetService.GetContentAsync(
            new ImageAssetId(id),
            cancellationToken);
    
        if (content is null)
        {
            return NotFound();
        }
    
        return File(
            content.Stream,
            content.ContentType,
            content.FileName);
    }

    private static ImageAssetDto MapToDto(ImageAsset asset)
    {
        return new ImageAssetDto(
            asset.Id.Value,
            asset.OriginalFileName,
            asset.StoredFileName,
            asset.ContentType,
            asset.SizeInBytes,
            asset.CreatedAt);
    }
}