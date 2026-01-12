using System.Collections;
using System.Text;
using DCTClass;
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
    
        if (file == null || file.Length == 0)
        {
            Console.Write("File is required");
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
            stream.Position = 0;
            MagickImage image = new MagickImage(stream);
            if (image.Width < 64 || image.Height < 1)
            {
                return BadRequest(new { error = "Image must be at least 64px wide and 1px tall" });
            }

            string text = "";
            if (fileExtension == ".jpg" || fileExtension == ".jpeg")
            {
                text = ClassDCT.Decrypt(stream);
            }
            else
            {
                BitArray bits = LSB.DecryptPNGImage(stream);
                byte[] bytes = LSB.ToByteArray(bits);
                text = Encoding.UTF8.GetString(bytes);
            }
            
            Console.WriteLine(text);
            return Ok(text);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(new { error = $"Invalid image file: {ex.Message}" });
        }
    }
}