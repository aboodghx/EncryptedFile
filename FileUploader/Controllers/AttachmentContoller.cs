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
            await videoFile.CopyToAsync(stream, cancellationToken);
        }

        string outputDirectory = @"C:\Uploads\Encrypted";
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        string encryptedFileName = Path.GetFileNameWithoutExtension(tempFilePath) + "_encrypted" + Path.GetExtension(tempFilePath);
        string encryptedFilePath = Path.Combine(outputDirectory, encryptedFileName);

        string password = videoPassword;
        fileServies.EncryptFile(tempFilePath, encryptedFilePath, password);

        return Ok(new { EncryptedFilePath = encryptedFilePath });
    }

    [HttpPost("decrypt")]
    public IActionResult DecryptVideo(string encryptedFilePath, string videoPassword)
    {
        try
        {
            fileServies.DecryptFileToUploads(encryptedFilePath, videoPassword);

            string decryptedFileName = Path.GetFileNameWithoutExtension(encryptedFilePath).Replace(".enc", "") + ".mp4";
            string decryptedFilePath = Path.Combine(@"C:\Uploads\Decrypted", decryptedFileName);

            return Ok(new { DecryptedFilePath = decryptedFilePath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
