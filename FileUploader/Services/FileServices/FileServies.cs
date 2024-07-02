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
            using (var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                inputStream.CopyTo(cryptoStream);
            }
        }
    }
}