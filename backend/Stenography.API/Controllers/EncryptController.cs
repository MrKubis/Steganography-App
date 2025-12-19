using ImageMagick;
using LSBClass;
using DCTClass;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers;

[ApiController]
[Route("api")]
public class EncryptController : ControllerBase
{
    [HttpPost("encrypt")]
    public async Task<IActionResult> Encrypt(IFormFile file, [FromForm] string message)
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

        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest(new { error = "Message is required" });
        }

        try
        {
            using var stream = file.OpenReadStream();
            using (var image = new MagickImage(stream))
            {
                if (image.Width < 64 || image.Height < 1)
                {
                    return BadRequest(new { error = "Image must be at least 64px wide and 1px tall" });
                }
            }
            byte[] result;
            if (fileExtension == ".jpg" || fileExtension == ".jpeg")
            {
                result = ClassDCT.Encrypt(stream,message);
            }
            else
            {
                result = LSB.EncryptPNGImage(stream, message);
            }
            Console.WriteLine();
            Console.WriteLine("File size: " + result.Length);
            return File(result, "image/png", "encrypted.png");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Invalid image file: {ex.Message}" });
        }
    }
}