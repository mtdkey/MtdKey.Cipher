namespace MtdKey.Cipher
{
    using Microsoft.Extensions.Options;
    using System.Security.Cryptography;

    public class AesManager : IAesManager
    {
        private readonly AesOptions aesOptions;
        
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
        public AesManager(IOptions<AesOptions> options)
        {
            aesOptions = options.Value;
        }

        public string EncryptModel<TModel>(TModel model) where TModel : class 
        {
            if(model == null) return string.Empty;
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

    }
}


