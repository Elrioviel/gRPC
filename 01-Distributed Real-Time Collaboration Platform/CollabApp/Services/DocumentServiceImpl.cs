using CollabApp.Protos;
using CollabApp.Server.Context;
using CollabApp.Server.Models;
using CollabApp.Server.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CollabApp.Server.Services
{
    public class DocumentServiceImpl : DocumentService.DocumentServiceBase
    {
        private readonly AppDbContext _db;
        private static readonly ConcurrentDictionary<string, List<IServerStreamWriter<EditMessage>>> _streams = new();

        public DocumentServiceImpl(AppDbContext db)
        {
            _db = db;
        }

        public override async Task<CreateDocumentReply> CreateDocument(CreateDocumentRequest request, ServerCallContext context)
        {
            var doc = new Document
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            _db.Documents.Add(doc);
            _db.Versions.Add(new DocumentVersion
            {
                Id = Guid.NewGuid(),
                DocumentId = doc.Id,
                ContentSnapshot = doc.Content,
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            return new CreateDocumentReply
            {
                DocumentId = doc.Id.ToString(),
                CreatedAt = Timestamp.FromDateTime(doc.CreatedAt.ToUniversalTime())
            };
        }

        public override async Task<GetDocumentReply> GetDocument(GetDocumentRequest request, ServerCallContext context)
        {
            var docId = Guid.Parse(request.DocumentId);
            var doc = await _db.Documents.FindAsync(docId);

            if (doc == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Document not found"));

            return new GetDocumentReply
            {
                DocumentId = doc.Id.ToString(),
                Title = doc.Title,
                Content = doc.Content,
                OwnerId = doc.OwnerId,
                UpdatedAt = Timestamp.FromDateTime(doc.UpdatedAt.ToUniversalTime())
            };
        }

        public override async Task<ListDocumentsReply> ListDocuments(ListDocumentsRequest request, ServerCallContext context)
        {
            var docs = await _db.Documents
                .Where(d => d.OwnerId == request.UserId)
                .ToListAsync();

            var reply = new ListDocumentsReply();
            reply.Documents.AddRange(docs.Select(doc => new GetDocumentReply
            {
                DocumentId = doc.Id.ToString(),
                Title = doc.Title,
                Content = doc.Content,
                OwnerId = doc.OwnerId,
                UpdatedAt = Timestamp.FromDateTime(doc.UpdatedAt.ToUniversalTime())
            }));

            return reply;
        }

        public override async Task JoinDocument(IAsyncStreamReader<EditMessage> requestStream, IServerStreamWriter<EditMessage> responseStream, ServerCallContext context)
        {
            var docId = "";
            _streams.TryAdd(docId, new());

            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    docId = message.DocumentId;

                    if (!_streams.ContainsKey(docId))
                        _streams.TryAdd(docId, new());

                    foreach (var stream in _streams[docId].ToList())
                    {
                        if (stream != responseStream)
                        {
                            await stream.WriteAsync(message);
                        }
                    }

                    if (!_streams[docId].Contains(responseStream))
                        _streams[docId].Add(responseStream);
                }
            }
            catch (Exception)
            {
                if (_streams.TryGetValue(docId, out var list))
                    list.Remove(responseStream);
            }
        }
    }
}