# MTD Key Cipher 
<a href="https://www.nuget.org/packages/MtdKey.Cipher">Nuget Package 2.0.1</a> 
# 🔐 Secure Encrypted Message Exchange Library

## Overview
This library provides a **robust solution** for exchanging encrypted messages between applications that share a predefined object structure. It is ideal for **microservices**, **remote backend servers**, **APIs**, and other distributed systems where structured data integrity is crucial.

By ensuring that both sender and receiver applications recognize the **data format**, this solution enables seamless integration, **secure token validation**, and **efficient object conversion**, reducing complexity in multi-application environments.

## 🔹 Key Features
- **IV Embedded in Encrypted Data** → Unlike conventional methods that send the **Initialization Vector (IV)** separately, this approach embeds the IV **within** the encrypted message, streamlining transmission and processing.
- **Object Conversion & Token Lifetime Validation** → Converts structured objects into encrypted tokens while automatically validating expiration, ensuring tokens remain usable **only within a predefined timeframe**.
- **Supports Primitive Types & Arrays** → While **nested objects** are **not supported**, the system securely handles **primitive types** (`string`, `int`, `bool`, etc.) and arrays/lists for structured data exchange.

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

      public IActionResult OnGet()
      {
            var tokenModel = new TestTokenModel()
            {
                UserName = "John Doe",
                Password = "securekey",
                Items = ["first", "second"]
            }; 

            var encryptedData = aesManager.EncryptStrongToken(tokenModel, TimeSpan.FromSeconds(60));
            if (ValidateStrongToken(encryptedData))
            {
                var decryptedModel = DecryptStrongToken.DecryptModel(encryptedData);
            }}      
        ....
      }

  ....

  }

```

> You can also use extensions for the Aes library

```cs

var tokenModel = new TestTokenModel()
{
    UserName = "John Doe",
    Password = "password",
    Items = [ "first", "second" ] 
};

var secretKey = AesCore.GenerateSecretKey();


using Aes aes = Aes.Create();

// It's an extension of the System.Security.Cryptography.Aes class
var tokenEncrypted = aes.EncryptStrongToken(tokenModel, secretKey, TimeSpan.FromSeconds(60));
var tokenDecrypted = aes.DecryptStrongToken<TestTokenModel>(tokenEncrypted, secretKey);

Console.WriteLine($"Decrypted User: {tokenDecrypted.UserName}");
Console.WriteLine($"Decrypted Password: {tokenDecrypted.Password}");
Console.WriteLine($"Decrypted Items: {string.Join(", ", tokenDecrypted.Items)}");

```

## 🛠 Installation
Clone the repository and ensure you have **.NET 9 ** installed:
```bash
git clone https://github.com/mtdkey/MtdKey.Cipher.git
cd your-repository
```

Examples of usage are located in the Tests, Api and Web folders.

| Folder        | Description                    |
| ------------- | -------------------------------|
| src           | The library source code.           |
| grpc          | An example gRPC server that encrypts and decrypts text messages can be used as a microservice. |
| api           | An example API application for creating complex secret tokens. |
| web           | Demo web application for creating unique tokens for each request of the same object. |
| tests         | Tests for this solution.  (xUnit) |

## License    
Copyright (c) – presented by [Oleg Bruev](https://github.com/olegbruev/).  
MTDKey Cipher is free and open-source software licensed under the MIT License.
