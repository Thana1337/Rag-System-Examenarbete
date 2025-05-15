using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.ServiceBus;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UploadService.Models;
using Common;

namespace UploadService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly BlobServiceClient _blobService;
        private readonly AzureStorageSettings _storageSettings;
        private readonly ServiceBusSender _sender;

        public DocumentsController(
            BlobServiceClient blobService,
            IOptions<AzureStorageSettings> storageOptions,
            ServiceBusSender sender)
        {
            _blobService = blobService;
            _storageSettings = storageOptions.Value;
            _sender = sender;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            var file = request.File;
            if (file is null || file.Length == 0)
                return BadRequest("Ingen fil mottagen eller filen är tom.");

            // 1) Ladda upp till Azure Blob
            var container = _blobService.GetBlobContainerClient(_storageSettings.ContainerName);
            await container.CreateIfNotExistsAsync();

            var documentId = Guid.NewGuid().ToString();
            var blobName = documentId + Path.GetExtension(file.FileName);
            var blobClient = container.GetBlobClient(blobName);
            await using (var stream = file.OpenReadStream())
                await blobClient.UploadAsync(stream, overwrite: true);

            // 2) Publicera DocumentUploadedEvent till Service Bus-queue
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
            await _sender.SendMessageAsync(message);

            // 3) Returnera svar
            var response = new UploadResponseDto
            {
                DocumentId = documentId,
                Message = "Filen är uppladdad och eventet skickat."
            };
            return Ok(response);
        }
    }
}
