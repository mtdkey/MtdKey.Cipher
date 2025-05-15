using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        [Obsolete]
        string EncryptModel<TModel>(TModel model) where TModel : class;

        /// <summary>
        /// Decryption of a string token encrypted with the <see cref="IAesManager.EncryptModel{TModel}(TModel)"/> method 
        /// </summary>
        /// <param name="token">Encrypted string token.</param>
        /// <returns>The custom instance class object.</returns>
        [Obsolete]
        TModel DecryptModel<TModel>(string token) where TModel : class, new();

        /// <summary>
        /// Encrypting a class object from the MtdKey.Cipher library <see cref="TokenModel"/>.
        /// </summary>
        /// <param name="model">The object of class <see cref="TokenModel"/></param>
        /// <returns>Encrypted string token.</returns>
        [Obsolete]
        string EncryptModel(TokenModel model);

        /// <summary>
        /// Decryption the string token encrypted with the <see cref="IAesManager.EncryptModel(TokenModel)"/> method 
        /// </summary>
        /// <param name="token">Encrypted string token.</param>
        /// <returns>The decrypted object kind of <see cref="TokenModel"/> class</returns>
        [Obsolete]
        TokenModel DecryptModel(string token);

        /// <summary>
        /// Encrypts an object into a strong token with an embedded expiration time.
        /// </summary>
        /// <typeparam name="TModel">The type of object to encrypt. Must be a class with a parameterless constructor.</typeparam>
        /// <param name="model">The object instance to be encrypted.</param>
        /// <param name="timeSpan">The duration for which the token remains valid.</param>
        /// <returns>Encrypted string token with expiration metadata.</returns>
        string EncryptStrongToken<TModel>(TModel model, TimeSpan timeSpan) where TModel : class, new();

        /// <summary>
        /// Validates whether a given encrypted token is still valid based on its expiration timestamp.
        /// </summary>
        /// <param name="token">The encrypted string token.</param>
        /// <returns>True if the token is valid and not expired; otherwise, false.</returns>
        bool ValidateStrongToken(string token);

        /// <summary>
        /// Decrypts a strong token back into its original object form.
        /// </summary>
        /// <typeparam name="TModel">The type of object being decrypted. Must be a class with a parameterless constructor.</typeparam>
        /// <param name="token">The encrypted string token to decrypt.</param>
        /// <returns>The decrypted object of type <typeparamref name="TModel"/>.</returns>
        TModel DecryptStrongToken<TModel>(string token) where TModel : class, new();

    }
}
