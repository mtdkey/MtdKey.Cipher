using System.Security.Cryptography;
using System.Text.Json;

namespace MtdKey.Cipher
{
    public static partial class AesCore
    {
        private static Dictionary<string, object?> ReadProperties<TModel>(TModel model) where TModel : class
        {
            ArgumentNullException.ThrowIfNull(model);

            var properties = typeof(TModel).GetProperties();
            var result = new Dictionary<string, object?>();

            foreach (var property in properties)
            {
                result[property.Name] = property.GetValue(model);
            }

            return result;
        }

        private static TModel CreateModel<TModel>(Dictionary<string, object?> data) where TModel : class, new()
        {
            ArgumentNullException.ThrowIfNull(data);

            TModel model = new();

            foreach (var kvp in data)
            {
                var property = typeof(TModel).GetProperty(kvp.Key);
                if (property != null && property.CanWrite)
                {
                    if (kvp.Value is JsonElement jsonElement)
                    {
                        if (jsonElement.ValueKind == JsonValueKind.Array)
                        {
                            // Convert JSON array to a list of strings
                            var list = jsonElement.EnumerateArray().Select(e => e.GetString()).ToList();

                            // If the target type is an array, convert the list to an array
                            if (property.PropertyType.IsArray)
                            {
                                Type elementType = property.PropertyType.GetElementType()!;
                                Array array = Array.CreateInstance(elementType, list.Count);
                                for (int i = 0; i < list.Count; i++)
                                {
                                    array.SetValue(Convert.ChangeType(list[i], elementType), i);
                                }
                                property.SetValue(model, array);
                            }
                            else
                            {
                                property.SetValue(model, list);
                            }
                        }
                        else
                        {
                            string? jsonString = jsonElement.ValueKind == JsonValueKind.String ? jsonElement.GetString() : jsonElement.ToString();
                            Type targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object? safeValue = jsonString == null ? null : Convert.ChangeType(jsonString, targetType);
                            property.SetValue(model, safeValue);
                        }
                    }


                }
            }

            return model;
        }

        private static string DecryptTokenToJson(Aes aes, string token, string key, int keySize = 256)
        {
            var tokenSchema = SplitToken(token);
            var iv = tokenSchema.IV;
            var cipherText = tokenSchema.Data;
            if (string.IsNullOrEmpty(iv) || string.IsNullOrEmpty(cipherText))
            {
                throw new Exception("Wrong token!");
            }
            byte[] buffer = Convert.FromBase64String(cipherText);
            aes.KeySize = keySize;
            aes.Key = Convert.FromBase64String(key);
            aes.IV = Convert.FromBase64String(iv);
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new(buffer);
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new(cryptoStream);
            string jsonText = streamReader.ReadToEnd();

            return jsonText;
        }

    }
}
