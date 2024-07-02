namespace FileUploader.Services.FileServices;

public interface IFileServies
{
    void EncryptFile(string inputFile, string outputFile, string password);
}