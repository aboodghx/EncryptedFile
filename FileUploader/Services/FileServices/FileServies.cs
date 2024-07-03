using System.Security.Cryptography;

namespace FileUploader.Services.FileServices;

public class FileServies : IFileServies
{
    public void EncryptFile(string inputFile, string outputFile, string password)
    {
        byte[] salt = new byte[16];

        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 10000);

        byte[] key = keyDerivationFunction.GetBytes(32);

        byte[] iv = keyDerivationFunction.GetBytes(16);

        using (var aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;

            using (var inputStream = File.OpenRead(inputFile))
            using (var outputStream = File.Create(outputFile))
            {
                outputStream.Write(salt, 0, salt.Length);

                using (var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    inputStream.CopyTo(cryptoStream);
                }
            }
        }
    }

    public void DecryptFileToUploads(string inputFile, string password)
    {
        string outputDirectory = @"C:\Uploads\Decrypted";

        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        string decryptedFileName = Path.GetFileNameWithoutExtension(inputFile).Replace(".enc", "") + ".mp4";
        string outputFile = Path.Combine(outputDirectory, decryptedFileName);

        byte[] salt = new byte[16];

        using (var inputStream = File.OpenRead(inputFile))
        {
            inputStream.Read(salt, 0, salt.Length);

            var keyDerivationFunction = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] key = keyDerivationFunction.GetBytes(32);
            byte[] iv = keyDerivationFunction.GetBytes(16);

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var outputStream = File.Create(outputFile))
                {
                    cryptoStream.CopyTo(outputStream);
                }
            }
        }
    }
}