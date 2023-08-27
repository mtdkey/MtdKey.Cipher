

namespace MtdKey.Cipher.Tests
{
    public class TestTokenModel
    {
        public string UserName { get; set; } = "John Doe";
        public string Password { get; set; } = "password";
        public List<string> Items { get; set; } = new() { 
            "first", "second"
        };
    }
}
