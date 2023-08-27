using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace MtdKey.Cipher
{
    public static class AesCore
    {
        public static string GenerateSecretKey(int keySize = 256)
        {
            using Aes aes = Aes.Create();
            aes.KeySize = keySize;           
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        public static string EncryptModel<TModel>(this Aes aes, TModel model, string key, int keySize = 256) where TModel : class
        {            
            var plainText = JsonSerializer.Serialize(model);
            byte[] array;

            aes.KeySize = keySize;
            aes.GenerateIV();

            aes.Key = Convert.FromBase64String(key);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
            {
                streamWriter.Write(plainText);
            }

            array = memoryStream.ToArray();

            var result = $"{Convert.ToBase64String(aes.IV)}/{Convert.ToBase64String(array)}";
            return Base64UrlEncoder.Encode(result);
        }

        public static TModel DecryptModel<TModel>(this Aes aes, string token, string key, int keySize = 256) where TModel : class, new()
        {
            TModel result;

            token = Base64UrlEncoder.Decode(token);
            int pos = token.IndexOf("=/");
            if (pos == -1) { throw new Exception("Wrong token!"); }
            var iv = token[..(pos + 1)];
            var cipherText = token[(pos + 2)..];

            byte[] buffer = Convert.FromBase64String(cipherText);
            aes.KeySize = keySize;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);

            string jsonText = streamReader.ReadToEnd();
            result = JsonSerializer.Deserialize<TModel>(jsonText) ?? new();

            return result;
        }

    }
}
