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
                        property.SetValue(model, ConvertJsonElement(jsonElement, property.PropertyType));
                    }
                }
            }

            return model;
        }

        private static object? ConvertJsonElement(JsonElement jsonElement, Type propertyType)
        {
            if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                return ConvertJsonArray(jsonElement, propertyType);
            }

            string? jsonString = jsonElement.ValueKind == JsonValueKind.String ? jsonElement.GetString() : jsonElement.ToString();
            Type targetType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            return ConvertJsonValue(jsonString, targetType);
        }

        private static object? ConvertJsonArray(JsonElement jsonElement, Type propertyType)
        {
            var list = jsonElement.EnumerateArray().Select(e => e.GetString()).ToList();

            if (propertyType.IsArray)
            {
                Type elementType = propertyType.GetElementType()!;
                Array array = Array.CreateInstance(elementType, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(Convert.ChangeType(list[i], elementType), i);
                }
                return array;
            }

            return list;
        }

        private static object? ConvertJsonValue(string? jsonString, Type targetType)
        {
            if (jsonString == null) return null;

            if (targetType == typeof(Guid))
            {
                return Guid.Parse(jsonString);
            }
            if (targetType == typeof(DateTime))
            {
                return DateTime.Parse(jsonString, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
            }
            if (targetType == typeof(bool))
            {
                return bool.Parse(jsonString);
            }
            if (targetType == typeof(int))
            {
                return int.Parse(jsonString);
            }
            if (targetType == typeof(long))
            {
                return long.Parse(jsonString);
            }
            if (targetType == typeof(decimal))
            {
                return decimal.Parse(jsonString);
            }
            if (targetType == typeof(float))
            {
                return float.Parse(jsonString);
            }
            return Convert.ChangeType(jsonString, targetType);
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
