using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace App.API.Controllers;

[ApiController]
[Route("api")]
public class DecryptController : ControllerBase
{
    [HttpPost("decrypt")]
    public async Task<IActionResult> Decrypt(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "File is required" });
        }

        var allowedExtensions = new[] { ".png", ".jpeg", ".jpg", ".bmp", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(fileExtension))
        {
            return BadRequest(new { error = "Invalid file type. Accepted types: png, jpeg, bmp, gif" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            using var image = await Image.LoadAsync(stream);
            
            if (image.Width < 64 || image.Height < 1)
            {
                return BadRequest(new { error = "Image must be at least 64px wide and 1px tall" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Invalid image file: {ex.Message}" });
        }

        // TODO: Add decryption logic here

        return Ok(new { message = "Decryption endpoint - logic not implemented yet" });
    }
}