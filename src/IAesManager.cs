using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtdKey.Cipher
{
    public interface IAesManager
    {
        /// <summary>
        /// Creates an encrypted string from some custom instance class object.
        /// </summary>
        /// <param name="model">The custom instance class object.</param>
        /// <returns>Encrypted string token.</returns>
        public string EncryptModel<TModel>(TModel model) where TModel : class;

        /// <summary>
        /// Decryption of a string token encrypted with the <see cref="IAesManager.EncryptModel{TModel}(TModel)"/> method 
        /// </summary>
        /// <param name="token">Encrypted string token.</param>
        /// <returns>The custom instance class object.</returns>
        public TModel DecryptModel<TModel>(string token) where TModel : class, new();

        /// <summary>
        /// Encrypting a class object from the MtdKey.Cipher library <see cref="TokenModel"/>.
        /// </summary>
        /// <param name="model">The object of class <see cref="TokenModel"/></param>
        /// <returns>Encrypted string token.</returns>
        public string EncryptModel(TokenModel model);

        /// <summary>
        /// Decryption the string token encrypted with the <see cref="IAesManager.EncryptModel(TokenModel)"/> method 
        /// </summary>
        /// <param name="token">Encrypted string token.</param>
        /// <returns>The decrypted object kind of <see cref="TokenModel"/> class</returns>
        public TokenModel DecryptModel(string token);
    }
}
