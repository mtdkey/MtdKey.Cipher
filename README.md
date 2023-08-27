
# MTD Key Cipher  [<img alt="NuGet" src="https://img.shields.io/nuget/v/MtdKey.Storage"/>](https://www.nuget.org/packages/mtdkey.storage/) <img alt="Licence MIT" src="https://img.shields.io/badge/licence-MIT-green"> <img alt="Platform" 

### The library for exchanging encrypted messages between different applications.

> Allows messages to be exchanged kind of class objects.

```cs

  var tokenModel = new TestTokenModel()
  {
      UserName = "John Doe",
      Password = "password",
      Items = new() { "first", "second" }
  };

  var secretKey = AesCore.GenerateSecretKey();

  //It is an extension of the System.Security.Cryptography.Aes class
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

  ....

  }

```

Examples of usage are located in the Tests or Web folders.

## License    
Copyright (c) – presented by [Oleg Bruev](https://github.com/olegbruev/).  
MTDKey Cipher is free and open-source software licensed under the MIT License.
