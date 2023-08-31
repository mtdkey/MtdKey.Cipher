using Microsoft.Extensions.Options;
using MtdKey.Cipher.Api.Controllers;
using System.Text.Json;

namespace MtdKey.Cipher.Tests
{
    public class ApiTests
    {

        [Theory]
        [InlineData("!2#4%^7*()'''\"\"\"\"", false)]
        [InlineData("{ \"jsonData\": \"!2#4%^7*()'''\"}", true)]
        public void TokenControllerTest(string data, bool jsonType)
        {
            var secretKey = AesCore.GenerateSecretKey();
            var aesOptions = Options.Create(new AesOptions()
            {
                SecretKey = secretKey,
                KeySize = 256
            });

            AesManager aesManager = new(aesOptions);

            var tokenController = new TokenController(aesManager);
            var encryptedToken = tokenController.GetToken(data);


            Assert.NotNull(encryptedToken);
            Assert.True(encryptedToken != string.Empty);

            var decryptedData = tokenController.GetModel(encryptedToken);

            if (jsonType)
            {                
                var actualObject = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
                var expectedObject = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedData);
                Assert.Equal(expectedObject?.FirstOrDefault().Value, actualObject?.FirstOrDefault().Value);
            }
            else
            {
                Assert.Equal(decryptedData, data);
            }
        }

    }
}
