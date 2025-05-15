using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;

namespace MtdKey.Cipher
{
    public static partial class AesCore
    {
        private const string EXPIRATION = "43E89ABB9429";
        public static string GenerateSecretKey(int keySize = 256)
        {
            using Aes aes = Aes.Create();
            aes.KeySize = keySize;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }

        public static string GenerateIV()
        {
            using Aes aes = Aes.Create();
            aes.GenerateIV();
            return Convert.ToBase64String(aes.IV);
        }

        [Obsolete]
        public static string EncryptModel<TModel>(this Aes aes, TModel model, byte[] iv, string key, int keySize = 256) where TModel : class
        {
            var plainText = JsonSerializer.Serialize(model);
            byte[] array;

            aes.KeySize = keySize;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = iv;

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
        [Obsolete]
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
        [Obsolete]
        public static TModel DecryptModel<TModel>(this Aes aes, string token, string key, int keySize = 256) where TModel : class, new()
        {
            TModel result;

            var tokenSchema = SplitToken(token);

            byte[] buffer = Convert.FromBase64String(tokenSchema.Data);
            aes.KeySize = keySize;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(tokenSchema.IV);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);

            string jsonText = streamReader.ReadToEnd();
            result = JsonSerializer.Deserialize<TModel>(jsonText) ?? new();

            return result;
        }


        public static string EncryptStrongToken<TModel>(this Aes aes, TModel model, string key, TimeSpan timeSpan, int keySize = 256) where TModel : class, new()
        {
            var properties = ReadProperties(model);
            properties[EXPIRATION] = DateTime.UtcNow.Add(timeSpan).ToString("yyyy-MM-ddTHH:mm:ssZ");

            var plainText = JsonSerializer.Serialize(properties);
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


        public static bool ValidateStrongToken(this Aes aes, string token, string key, int keySize = 256)
        {
            string jsonText = DecryptTokenToJson(aes, token, key, keySize);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object?>>(jsonText) ?? [];

            DateTime expirationTime = DateTime.MinValue; // Initialize with a default value

            if (dictionary.TryGetValue(EXPIRATION, out var expiration) && expiration is JsonElement jsonElement)
            {
                string expirationString = jsonElement.GetString() ?? string.Empty;
                expirationTime = DateTime.ParseExact(expirationString, "yyyy-MM-ddTHH:mm:ssZ",
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            }

            return DateTime.UtcNow <= expirationTime;
        }


        public static TModel DecryptStrongToken<TModel>(this Aes aes, string token, string key, int keySize = 256) where TModel : class, new()
        {
            string jsonText = DecryptTokenToJson(aes, token, key, keySize);
            var dictionary  = JsonSerializer.Deserialize<Dictionary<string,object?>>(jsonText) ?? [];
            dictionary.Remove(EXPIRATION);
            var model = CreateModel<TModel>(dictionary);
            
            return model;
        }
        

        public static TokenSchema SplitToken(string token)
        {
            token = Base64UrlEncoder.Decode(token);
            int pos = token.IndexOf("=/");
            if (pos == -1) { throw new Exception("Wrong token!"); }
            var iv = token[..(pos + 1)];
            var cipherText = token[(pos + 2)..];

            return new TokenSchema { IV = iv, Data = cipherText };
        }
    }
}
