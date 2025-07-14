using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class EncryptionService
    {
        // Recomendado: colocar essa chave no appsettings ou secrets.json
        private static readonly string Key = "MyUltraSecretKey@2025!"; // Deve ter 32 bytes (256 bits)

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key.PadRight(32)); // Garantir 32 bytes
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV();
            var iv = aes.IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, iv);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var result = new byte[iv.Length + cipherBytes.Length];
            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(cipherBytes, 0, result, iv.Length, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string encrypted)
        {
            var fullCipher = Convert.FromBase64String(encrypted);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key.PadRight(32));
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        // Verifica se a senha fornecida corresponde ao hash armazenado
        public static bool VerifyPassword(string passwordTyped, string salt, string encryptedPassword)
        {
            var passWithSalt = passwordTyped + salt;
            var dencryptedPassword = EncryptionService.Decrypt(encryptedPassword);
            return dencryptedPassword == passWithSalt;
        }

    }
}
