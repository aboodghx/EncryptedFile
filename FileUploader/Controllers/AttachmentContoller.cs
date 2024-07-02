using FileUploader.Services.FileServices;
using Microsoft.AspNetCore.Mvc;

namespace FileUploader.Controllers;

public class AttachmentContoller : ControllerBase
{
    private readonly IFileServies fileServies;

    public AttachmentContoller(IFileServies fileServies)
    {
        this.fileServies = fileServies;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadVideo(IFormFile videoFile, string videoPassword, CancellationToken cancellationToken)
    {
        var tempFilePath = Path.GetTempFileName();
        using (var stream = new FileStream(tempFilePath, FileMode.Create))
        {
            videoFile.CopyTo(stream);
        }

        string encryptedFilePath = Path.GetTempFileName();
        string password = videoPassword;
        fileServies.EncryptFile(tempFilePath, encryptedFilePath, password);

        return Ok();
    }
}
