using System.Text.Json.Nodes;

namespace MtdKey.Cipher.Api
{
    public class TokenData
    {
        public bool JsonType { get; set; } = true;
        public JsonObject JsonData { get; set; } = new JsonObject();        
    }
}
