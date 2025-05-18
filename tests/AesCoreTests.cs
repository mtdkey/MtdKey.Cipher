using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Xunit.Abstractions;

namespace MtdKey.Cipher.Tests
{
    public class AesCoreTests
    {
        private readonly ITestOutputHelper outputHelper;
        public AesCoreTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void GenerateSecretKeyTest()
        {
            var secretKey = AesCore.GenerateSecretKey();
            outputHelper.WriteLine(secretKey);
        }

        [Fact]
        public void EncryptAndDecryptModelTest()
        {
            var tokenModel = new TestTokenModel()
            {
                UserName = "John Doe",
                Password = "password",
                Items = new() { "first", "second" }
            };
            var secretKey = AesCore.GenerateSecretKey();

            using Aes aes = Aes.Create();
            var tokenEncrypted = aes.EncryptModel(tokenModel, secretKey);
            Assert.NotNull(tokenEncrypted);

            var tokenDecrypted = aes.DecryptModel<TestTokenModel>(tokenEncrypted, secretKey);
            Assert.NotNull(tokenDecrypted);

            Assert.True(tokenModel?.UserName.Equals(tokenDecrypted.UserName));
            Assert.True(tokenModel?.Password.Equals(tokenDecrypted.Password));
            Assert.True(tokenModel?.Items[0].Equals(tokenDecrypted.Items[0]));
        }

        [Fact]
        public void EncryptStrongToken_ShouldReturnValidEncryptedString()
        {
            // Arrange
            var tokenModel = new TestTokenModel()
            {
                UserName = "John Doe",
                Password = "password",
                Items = ["first", "second"]
            };

            TimeSpan timeSpan = TimeSpan.FromHours(1);
            var secretKey = AesCore.GenerateSecretKey();
            using Aes aes = Aes.Create();

            // Act
            var encryptedString = aes.EncryptStrongToken(tokenModel, secretKey, timeSpan);

            // Assert
            Assert.NotNull(encryptedString);
            Assert.NotEmpty(encryptedString);

            // Ensure expiration field is included
            var decryptedData = aes.DecryptStrongToken<TestTokenModel>(encryptedString, secretKey);

            Assert.True(tokenModel?.UserName.Equals(decryptedData.UserName));
            Assert.True(tokenModel?.Password.Equals(decryptedData.Password));
            Assert.True(tokenModel?.Items[0].Equals(decryptedData.Items[0]));
            Assert.True(tokenModel?.Guid.Equals(decryptedData.Guid));
            Assert.True(tokenModel?.IsValid.Equals(decryptedData.IsValid));
            Assert.True(tokenModel?.Expiration.Equals(decryptedData.Expiration));
        }

        [Fact]
        public void ValidateStrongToken_ShouldReturnTrue_ForValidToken()
        {
            // Arrange
            var tokenModel = new TestTokenModel()
            {
                UserName = "John Doe",
                Password = "securepassword",
                Items = ["first", "second"]
            };

            TimeSpan timeSpan = TimeSpan.FromHours(1);
            var secretKey = AesCore.GenerateSecretKey();
            using Aes aes = Aes.Create();

            // Encrypt token
            var encryptedString = aes.EncryptStrongToken(tokenModel, secretKey, timeSpan);

            // Act
            bool isValid = aes.ValidateStrongToken(encryptedString, secretKey);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateStrongToken_ShouldReturnFalse_ForExpiredToken()
        {
            // Arrange
            var tokenModel = new TestTokenModel()
            {
                UserName = "John Doe",
                Password = "securepassword",
                Items = ["first", "second"]
            };

            TimeSpan expiredTimeSpan = TimeSpan.FromHours(-1); // Token already expired
            var secretKey = AesCore.GenerateSecretKey();
            using Aes aes = Aes.Create();

            // Encrypt expired token
            var expiredToken = aes.EncryptStrongToken(tokenModel, secretKey, expiredTimeSpan);

            // Act
            bool isValid = aes.ValidateStrongToken(expiredToken, secretKey);

            // Assert
            Assert.False(isValid);
        }



        [Fact]
        public void EncryptDecryptPerformanceTest()
        {
            var items = new List<string>(Enumerable.Range(1, 100000).Select(i => $"Item-{i}")); // Large dataset
            string jsonData = JsonConvert.SerializeObject(items);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

            outputHelper.WriteLine($"JSON Size: {jsonBytes.Length} bytes");

            // Arrange: Generate large JSON data
            var largeData = new TestTokenModel()
            {
                UserName = "John Doe",
                Password = "password",
                Items = items
            };

            TimeSpan timeSpan = TimeSpan.FromHours(1);
            var secretKey = AesCore.GenerateSecretKey();
            using Aes aes = Aes.Create();

            // Measure encryption speed
            Stopwatch encryptWatch = Stopwatch.StartNew();
            var encryptedString = aes.EncryptStrongToken(largeData, secretKey, timeSpan, 128);
            encryptWatch.Stop();

            outputHelper.WriteLine($"Encryption Time: {encryptWatch.ElapsedMilliseconds} ms");

            // Measure decryption speed
            Stopwatch decryptWatch = Stopwatch.StartNew();
            var decryptedData = aes.DecryptStrongToken<TestTokenModel>(encryptedString, secretKey, 128);
            decryptWatch.Stop();

            outputHelper.WriteLine($"Decryption Time: {decryptWatch.ElapsedMilliseconds} ms");

            // Assertions
            Assert.NotNull(encryptedString);
            Assert.NotEmpty(encryptedString);
            Assert.NotNull(decryptedData);
            Assert.True(decryptedData.Items.Count == largeData.Items.Count);
        }

    }

}