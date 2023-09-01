using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace MtdKey.Cipher.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAesManager aesManager;

        private const string key = "key";

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
                jsonData = new JsonObject
                    {
                        { key, data }
                    };
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

            var jsonNode = model.JsonData.First(x => x.Key == key).Value;
            return jsonNode == null ? string.Empty : jsonNode.ToString();
        }

    }
}
