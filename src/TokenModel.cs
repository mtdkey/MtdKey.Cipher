using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtdKey.Cipher
{
    /// <summary>
    /// The class is used as a standard for exchange between applications that use <see cref="MtdKey.Cipher"/> library.
    /// Use <see cref="AesManager.EncryptModel(TokenModel)"/> and <see cref="AesManager.DecryptModel(string)"/> methods.
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// Get or set the token ID if you need to manage tokens.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// Get or set the date and time the token was created if you need to manage the lifetime of tokens.
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;
        /// <summary>
        /// Get or set the string data for encrypted data exchange.
        /// </summary>
        public string Data { get; set; } = string.Empty;
    }
}
