using CollabApp.Protos;
using Grpc.Core;
using System.Collections.Concurrent;

namespace CollabApp.Server.Services
{
    public class CollaborationServiceImpl : CollaborationService.CollaborationServiceBase
    {
        private static readonly ConcurrentDictionary<string, List<IServerStreamWriter<EditEvent>>> _stream = new ConcurrentDictionary<string, List<IServerStreamWriter<EditEvent>>>();

        public override async Task Edit (IAsyncStreamReader<EditEvent> requestStream, IServerStreamWriter<EditEvent> responseStream, ServerCallContext context)
        {
            var clientId = Guid.NewGuid().ToString();
            string? documentId = null;

            var user = context.GetHttpContext()?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(user))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing JWT"));

            try
            {
                await foreach (var edit in requestStream.ReadAllAsync())
                {
                    documentId = edit.DocumentId;

                    _stream.TryAdd(documentId, new List<IServerStreamWriter<EditEvent>>());

                    if (!_stream[documentId].Contains(responseStream))
                        _stream[documentId].Add(responseStream);

                    foreach (var stream in _stream[documentId])
                    {
                        if (stream != responseStream)
                        {
                            try
                            {
                                await stream.WriteAsync(edit);
                            }
                            catch (Exception ex) when (ex is RpcException || ex is OperationCanceledException)
                            {
                                Console.WriteLine("Stream cancelled or disconnected.");
                            }
                        }
                    }
                }
            }
            finally
            {
                if (documentId != null && _stream.ContainsKey(documentId))
                    _stream[documentId].Remove(responseStream);
            }
        }
    }
}
