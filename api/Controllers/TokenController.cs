using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace MtdKey.Cipher.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAesManager aesManager;

        public TokenController(IAesManager aesManager)
        {
            this.aesManager = aesManager;
        }

        /// <summary>
        /// The request to create a token 
        /// </summary>
        /// <param name="data">Plain text or json string</param>
        /// <returns></returns>
        [HttpGet("get/token/{data}")]
        public string GetToken(string data)
        {
            var jsonType = true;
            JsonObject jsonData;
            try
            {
                jsonData = JsonNode.Parse(data)?.AsObject() ?? new JsonObject();
            }

            catch (Exception)
            {
                jsonType = false;
                jsonData = CreateJsonObjectFromText(data);
            }

            TokenData model = new()
            {
                JsonType = jsonType,
                JsonData = jsonData
            };

            string token = aesManager.EncryptModel(model);
            return token;
        }

        [HttpGet("get/data/{token}")]
        public string GetModel(string token)
        {
            TokenData model = aesManager.DecryptModel<TokenData>(token);

            if (model.JsonType)
                return model.JsonData.ToString();

            var jsonText = model.JsonData.First(x => x.Key == "data").Value;
            return jsonText == null ? string.Empty : jsonText.ToString();
        }

        private static JsonObject CreateJsonObjectFromText(string text)
        {
            var jsonObject = new JsonObject();
            Dictionary<string, string> dictionary = new()
                {
                    { "data", text }
                };
            var jsonText = JsonSerializer.Serialize(dictionary);
            var jsonValue = JsonNode.Parse(jsonText);
            if (jsonValue != null)
            {
                jsonObject = jsonValue.AsObject();
            }

            return jsonObject;
        }

    }
}
