
# MTD Key Cipher 
<a href="https://www.nuget.org/packages/MtdKey.Cipher">Nuget Package 1.0.2</a> 

### The library for exchanging encrypted messages between different applications.

> <p>Allows messages to be exchanged kind of class objects.</p>
> Creates complex tokens that can be send over HTTP as hyperlinks and make APIs more flexible and secure.

```cs

  var tokenModel = new TestTokenModel()
  {
      UserName = "John Doe",
      Password = "password",
      Items = new() { "first", "second" }
  };

  var secretKey = AesCore.GenerateSecretKey();

  //It's an extension of the System.Security.Cryptography.Aes class
  using Aes aes = Aes.Create();
  var tokenEncrypted = aes.EncryptModel(tokenModel, secretKey);

  var tokenDecrypted = aes.DecryptModel<TestTokenModel>(tokenEncrypted, secretKey);
```

> The special AesManager class can be used as a dependent injection in the Asp.Net Web App.

```cs
  builder.Services.AddAesMangerService(options => {
      options.SecretKey = builder.Configuration["AesOptions:SecretKey"] ?? string.Empty;
      options.KeySize = int.Parse(builder.Configuration["AesOptions:KeySize"] ?? "256");
  });
```

```cs
  public class IndexModel : PageModel
  {
      private readonly IAesManager aesManager;

      public IndexModel(IAesManager aesManager)
      {
          this.aesManager = aesManager;
      }

      public IActionResult OnPost()
      {

        var encryptedData = aesManager.EncryptModel(tokenModel);
        var decryptedModel = aesManager.DecryptModel(encryptedData);
            
        ....
      }

  ....

  }

```

Examples of usage are located in the Tests, Api and Web folders.

## License    
Copyright (c) – presented by [Oleg Bruev](https://github.com/olegbruev/).  
MTDKey Cipher is free and open-source software licensed under the MIT License.
