using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
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

    }

}