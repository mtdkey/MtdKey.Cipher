using Grpc.Net.Client;
using MtdKey.Cipher.gRPC.Client;

// The port number must match the port of the gRPC server.
//Console.WriteLine("I'm ready!");
//Console.ReadLine();

    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

using var channel = GrpcChannel.ForAddress("https://localhost:7276",
    new GrpcChannelOptions { HttpHandler = handler });

var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(
                  new HelloRequest { Name = "GreeterClient" });

Console.WriteLine("Greeting: " + reply.Message);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

