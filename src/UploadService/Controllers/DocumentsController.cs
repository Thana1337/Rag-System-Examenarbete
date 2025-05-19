// DocumentsController.cs
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Common;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UploadService.Models;

namespace UploadService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly BlobServiceClient _blobService;
        private readonly AzureStorageSettings _storageSettings;
        private readonly ServiceBusSender _sender;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(
            BlobServiceClient blobService,
            IOptions<AzureStorageSettings> storageOptions,
            ServiceBusSender sender,
            ILogger<DocumentsController> logger)
        {
            _blobService = blobService;
            _storageSettings = storageOptions.Value;
            _sender = sender;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(List<UploadResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            var files = request.Files;
            if (files == null || files.Count == 0)
            {
                _logger.LogWarning("Upload called with no files.");
                return BadRequest("Ingen fil mottagen eller filerna är tomma.");
            }

            var container = _blobService.GetBlobContainerClient(_storageSettings.ContainerName);
            await container.CreateIfNotExistsAsync();

            var responses = new List<UploadResponseDto>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                {
                    _logger.LogWarning("Skipping empty file: {FileName}", file.FileName);
                    continue;
                }

                try
                {
                    // 1) Upload to Azure Blob
                    var documentId = Guid.NewGuid().ToString();
                    var blobName = documentId + Path.GetExtension(file.FileName);
                    var blobClient = container.GetBlobClient(blobName);

                    _logger.LogInformation("Uploading file '{FileName}' as blob '{BlobName}'", file.FileName, blobName);
                    await using (var stream = file.OpenReadStream())
                        await blobClient.UploadAsync(stream, overwrite: true);

                    // 2) Publish event
                    var @event = new DocumentUploadedEvent
                    {
                        DocumentId = documentId,
                        FileName = file.FileName,
                        FilePath = blobClient.Uri.ToString(),
                        UploadedAt = DateTime.UtcNow
                    };
                    var message = new ServiceBusMessage(JsonSerializer.Serialize(@event))
                    {
                        ContentType = "application/json"
                    };

                    _logger.LogInformation("Sending Service Bus message for document {DocumentId}", documentId);
                    await _sender.SendMessageAsync(message);

                    // 3) Collect response
                    responses.Add(new UploadResponseDto
                    {
                        DocumentId = documentId,
                        Message = $"Filen '{file.FileName}' är uppladdad och eventet skickat."
                    });

                    _logger.LogInformation("Successfully processed file '{FileName}'", file.FileName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process file '{FileName}'", file.FileName);
                    responses.Add(new UploadResponseDto
                    {
                        DocumentId = null,
                        Message = $"Fel vid uppladdning av '{file.FileName}'."
                    });
                }
            }

            return Ok(responses);
        }
    }
}
