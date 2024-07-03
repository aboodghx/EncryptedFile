namespace FileUploader.Services.FileServices;

public interface IFileServies
{
    void EncryptFile(string inputFile, string outputFile, string password);

    void DecryptFileToUploads(string inputFile, string password);
}