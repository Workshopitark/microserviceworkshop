using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;


[ApiController]
[Route("controller")]

public class ImageController : ControllerBase
{

    private string _imagePath = string.Empty;

    private readonly ILogger<ImageController> _logger;




    public ImageController(ILogger<ImageController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _imagePath = configuration["ImagePath"] ?? String.Empty;
    }


    [HttpPost("upload"), DisableRequestSizeLimit]
    public IActionResult UploadImage()
    {
        List<Uri> images = new List<Uri>();
        try
        {
            foreach (var formFile in Request.Form.Files)
            {
                // Validate file type and size
                // Validate file type and size

                if (formFile.ContentType != "image/jpeg" && formFile.ContentType != "image/png")
                {
                    return BadRequest($"Invalid file type for file {formFile.FileName}. Only JPEG and PNG files are allowed.");
                }
                if (formFile.Length > 1048576) // 1MB
                {
                    return BadRequest($"File {formFile.FileName} is too large. Maximum file size is 1MB.");
                }
                if (formFile.Length > 0)
                {
                    var fileName = "image-" + Guid.NewGuid().ToString() + ".jpg";
                    var fullPath = _imagePath + Path.DirectorySeparatorChar + fileName;

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    var imageURI = new Uri(fileName, UriKind.RelativeOrAbsolute);
                    images.Add(imageURI);
                }
                else
                {
                    return BadRequest("Empty file submited.");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error.");
        }
        return Ok(images);
    }

}