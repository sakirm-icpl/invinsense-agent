using System.IO;
using System.Security.Cryptography;

namespace Common.Encryption
{
    internal class FileEncryptDecrypt
    {
        private const int SaltSize = 8;

        public static void Encrypt(FileInfo targetFile, string password)
        {
            var keyGenerator = new Rfc2898DeriveBytes(password, SaltSize);
            var engine = Rijndael.Create();

            // BlockSize, KeySize in bit --> divide by 8
            engine.IV = keyGenerator.GetBytes(engine.BlockSize / 8);
            engine.Key = keyGenerator.GetBytes(engine.KeySize / 8);

            using (var fileStream = targetFile.Create())
            {
                // write random salt
                fileStream.Write(keyGenerator.Salt, 0, SaltSize);

                using (var cryptoStream = new CryptoStream(fileStream, engine.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    // write data
                }
            }
        }

        public static void Decrypt(FileInfo sourceFile, string password)
        {
            // read salt
            var fileStream = sourceFile.OpenRead();
            var salt = new byte[SaltSize];
            fileStream.Read(salt, 0, SaltSize);

            // initialize algorithm with salt
            var keyGenerator = new Rfc2898DeriveBytes(password, salt);
            var enggine = Rijndael.Create();
            enggine.IV = keyGenerator.GetBytes(enggine.BlockSize / 8);
            enggine.Key = keyGenerator.GetBytes(enggine.KeySize / 8);

            // decrypt
            using (var cryptoStream = new CryptoStream(fileStream, enggine.CreateDecryptor(), CryptoStreamMode.Read))
            {
                // read data
            }
        }
    }
}