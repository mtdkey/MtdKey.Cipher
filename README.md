
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

appsettings.json
```json
  "AesOptions": {
    "SecretKey": "[Your secret key]",
    "KeySize": "256"
  },
```
Program.cs
```cs
  builder.Services.AddAesMangerService(options => {
        options.SecretKey = builder.Configuration.GetValue<string>("AesOptions:SecretKey");
        options.KeySize = builder.Configuration.GetValue<int>("AesOptions:KeySize");
  });
```
Index.cshtml.cs
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

| Folder        | Description                    |
| ------------- | -------------------------------|
| src           | The library source code.           |
| grps          | An example gRPC server that encrypts and decrypts text messages can be used as a microservice - one secret key for multiple clients. |
| api           | An example API application for creating complex secret tokens. |
| web           | Demo web application for creating unique tokens for each request of the same object. |
| tests         | Tests for this solution.  (xUnit) |

## License    
Copyright (c) – presented by [Oleg Bruev](https://github.com/olegbruev/).  
MTDKey Cipher is free and open-source software licensed under the MIT License.
