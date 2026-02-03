using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace tutorial_4
{
    internal class Question_5
    {
        // Lưu ý: Trong thực tế KHÔNG hard-code key & IV như thế này!
        // Nên dùng key derivation (PBKDF2, Argon2) từ password + salt
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("ThisIs32ByteSecretKey1234567890ab"); // 32 bytes cho AES-256
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("16ByteIV12345678");               // 16 bytes

        public static void DemonstrateSecureFileStorage(string inputFilePath, string encryptedFilePath, string decryptedFilePath)
        {
            Console.WriteLine("=== Secure & Efficient File Storage Demo ===\n");

            try
            {
                // Bước 1: Encrypt → Compress → Save
                Console.WriteLine("Encrypting & compressing...");
                EncryptAndCompress(inputFilePath, encryptedFilePath);

                Console.WriteLine($"→ Saved encrypted file: {encryptedFilePath}");

                // Bước 2: Reverse – Decompress → Decrypt → Save
                Console.WriteLine("\nDecompressing & decrypting...");
                DecompressAndDecrypt(encryptedFilePath, decryptedFilePath);

                Console.WriteLine($"→ Restored file: {decryptedFilePath}");

                // So sánh nội dung gốc và khôi phục (tùy chọn)
                string original = File.ReadAllText(inputFilePath);
                string restored = File.ReadAllText(decryptedFilePath);
                bool success = original == restored;

                Console.WriteLine($"\nVerification: {(success ? "Success ✓" : "Failed ✗")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void EncryptAndCompress(string inputPath, string outputPath)
        {
            byte[] plainText = File.ReadAllBytes(inputPath);

            // Bước 1: Encrypt
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainText, 0, plainText.Length);
                    cs.FlushFinalBlock();
                    encrypted = ms.ToArray();
                }
            }

            // Bước 2: Compress encrypted data
            using (var outputStream = File.Create(outputPath))
            using (var gzip = new GZipStream(outputStream, CompressionMode.Compress))
            {
                gzip.Write(encrypted, 0, encrypted.Length);
            }
        }

        private static void DecompressAndDecrypt(string inputPath, string outputPath)
        {
            byte[] decompressed;

            // Bước 1: Decompress
            using (var inputStream = File.OpenRead(inputPath))
            using (var gzip = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var ms = new MemoryStream())
            {
                gzip.CopyTo(ms);
                decompressed = ms.ToArray();
            }

            // Bước 2: Decrypt
            byte[] decrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(decompressed, 0, decompressed.Length);
                    cs.FlushFinalBlock();
                    decrypted = ms.ToArray();
                }
            }

            File.WriteAllBytes(outputPath, decrypted);
        }
    }
}