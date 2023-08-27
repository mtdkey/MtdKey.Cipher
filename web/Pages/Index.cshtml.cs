using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MtdKey.Cipher.WebApp.Pages
{

    public class IndexModel : PageModel
    {
        private readonly IAesManager aesManager;

        public IndexModel(IAesManager aesManager)
        {
            this.aesManager = aesManager;
        }

        [BindProperty]
        public TokenModel TokenModel { get; set; } = new();
        [TempData]
        public string EncryptedData { get; set; } = string.Empty;
        [TempData]
        public string DecryptedData { get; set; } = string.Empty;

        public void OnGet(string Id, string dateTime, string textData = "Test Word")
        {
            if (Guid.TryParse(Id, out Guid guid))
                TokenModel.Id = guid;
            else TokenModel.Id = Guid.NewGuid();

            if (DateTime.TryParse(dateTime, out DateTime created))
                TokenModel.Created = created;
            else TokenModel.Created = DateTime.Now;
            TokenModel.Data = textData;
        }

        public IActionResult OnPost()
        {

            EncryptedData = aesManager.EncryptModel(TokenModel);
            var decryptedModel = aesManager.DecryptModel(EncryptedData);
            
            DecryptedData = $" ID: {decryptedModel.Id.ToString().ToUpper()} " +
                $"\n Created: {decryptedModel.Created} " +
                $"\n Data: {decryptedModel.Data}";

            return RedirectToPage("/Index", new { 
                TokenModel.Id,
                dateTime = TokenModel.Created,
                textData = TokenModel.Data 
            });
        }
    }
}