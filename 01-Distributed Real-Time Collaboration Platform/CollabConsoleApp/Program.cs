using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using CollabApp.Protos;
using CollabApp.Server.Protos;
using Grpc.Core;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Collab Console Client...");

        var channel = GrpcChannel.ForAddress("https://localhost:7287");
        var client = new DocumentService.DocumentServiceClient(channel);

        using var call = client.JoinDocument();

        _ = Task.Run(async () =>
        {
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Received: {message.UserId}: {message.Content}");
            }
        });

        var responseReaderTask = Task.Run(async () =>
        {
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Received: {message.UserId}: {message.Content}");
            }
        });

        while (true)
        {
            var content = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(content)) continue;

            var edit = new EditMessage
            {
                DocumentId = "doc1",
                UserId = "consoleUser",
                Content = content,
                Version = 1
            };

            await call.RequestStream.WriteAsync(edit);
        }
    }
}