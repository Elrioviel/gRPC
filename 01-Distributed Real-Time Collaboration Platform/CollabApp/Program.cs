using CollabApp.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CollaborationServiceImpl>();
app.MapGet("/", () => "This is a gRPC service. Use a gRPC client to connect.");

app.Run();
