

namespace MtdKey.Cipher.Tests
{
    public class TestTokenModel
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = "John Doe";
        public string Password { get; set; } = "password";
        public List<string> Items { get; set; } = new() { 
            "first", "second"
        };
        public DateTime Expiration { get; set; } = DateTime.UtcNow;
        public bool IsValid { get; set; } = true;
    }
}
