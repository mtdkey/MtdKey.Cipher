namespace MtdKey.Cipher
{
    public class AesOptions
    {
        /// <summary>
        /// The secret key for use to the symmetric algorithm Base64 string format.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;
        /// <summary>
        /// The key length for symmetric block cipher [128, 192 or 256] bits.
        /// </summary>
        public int KeySize { get; set; } = 256;
    }
}
