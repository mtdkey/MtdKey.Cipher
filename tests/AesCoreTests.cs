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

            Assert.True(tokenModel.UserName.Equals(tokenDecrypted.UserName));
            Assert.True(tokenModel.Password.Equals(tokenDecrypted.Password));
            Assert.True(tokenModel.Items[0].Equals(tokenDecrypted.Items[0]));
        }
    }
}