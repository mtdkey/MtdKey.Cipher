namespace MtdKey.Cipher
{
    using Microsoft.Extensions.Options;
    using System;
    using System.Reflection;
    using System.Security.Cryptography;

    /// <summary>
    /// The constructor is implemented for use with dependency injection.
    /// <see cref="ServiceExtensions.AddAesMangerService(Microsoft.Extensions.DependencyInjection.IServiceCollection, Action{AesOptions})"/>
    /// if you want to use <see cref="AesManager"/> without dependency injection then use the below code.
    /// <code>
    ///  var aesOptions = Options.Create(new AesOptions()
    ///  {
    ///      SecretKey = "[your secret key]",
    ///      KeySize = 256
    ///  });
    ///  AesManager aesManager = new(aesOptions);
    /// </code>
    /// </summary>
    public class AesManager(IOptions<AesOptions> options) : IAesManager
    {
        private readonly AesOptions aesOptions = options.Value;

        public string EncryptModel<TModel>(TModel model) where TModel : class
        {
            if (model == null) return string.Empty;
            using Aes aes = Aes.Create();
            var token = aes.EncryptModel(model, aesOptions.SecretKey, aesOptions.KeySize);
            return token;
        }

        public TModel DecryptModel<TModel>(string token) where TModel : class, new()
        {
            using Aes aes = Aes.Create();
            return aes.DecryptModel<TModel>(token, aesOptions.SecretKey, aesOptions.KeySize);
        }

        public string EncryptModel(TokenModel model)
        {
            using Aes aes = Aes.Create();
            var token = aes.EncryptModel(model, aesOptions.SecretKey, aesOptions.KeySize);
            return token;
        }

        public TokenModel DecryptModel(string token)
        {
            using Aes aes = Aes.Create();
            return aes.DecryptModel<TokenModel>(token, aesOptions.SecretKey, aesOptions.KeySize);
        }

        public string EncryptStrongToken<TModel>(TModel model, TimeSpan timeSpan) where TModel : class, new()
        {
            using Aes aes = Aes.Create();
            var token = aes.EncryptStrongToken(model, aesOptions.SecretKey, timeSpan, aesOptions.KeySize);
            return token;
        }

        public bool ValidateStrongToken(string token)
        {
            using Aes aes = Aes.Create();

            bool result;
            try
            {
                result = aes.ValidateStrongToken(token, aesOptions.SecretKey, aesOptions.KeySize);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public TModel DecryptStrongToken<TModel>(string token) where TModel : class, new()
        {
            using Aes aes = Aes.Create();
            return aes.DecryptStrongToken<TModel>(token, aesOptions.SecretKey, aesOptions.KeySize);
        }

    }
}


