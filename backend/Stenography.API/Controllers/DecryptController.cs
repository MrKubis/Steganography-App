using System.Collections;
using System.Text;
using ImageMagick;
using LSBClass;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using static System.Net.Mime.MediaTypeNames;

namespace App.API.Controllers;

[ApiController]
[Route("api")]
public class DecryptController : ControllerBase
{
    [HttpPost("decrypt")]
    public async Task<IActionResult> Decrypt(IFormFile file)
    {
        Console.WriteLine("jaja");

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
            MagickImage image = new MagickImage(stream);
            if (image.Width < 64 || image.Height < 1)
            {
                return BadRequest(new { error = "Image must be at least 64px wide and 1px tall" });
            }
            BitArray bits = LSB.DecryptPNGImage(stream);
            byte[] bytes = LSB.ToByteArray(bits);
            string text = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(text);
            return Ok(text);

        }
        catch (Exception ex)
        {
            return BadRequest(new { error = $"Invalid image file: {ex.Message}" });
        }
    }
}