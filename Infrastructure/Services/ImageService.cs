using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Contracts;
using Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Service
{
    public class ImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        public ImageService(BlobServiceClient blobServiceClient, IOptions<AzureBlobStorage> azureBlobStorage)
        {
            _blobServiceClient = blobServiceClient;
            _containerName = azureBlobStorage.Value.BlobContainerName;
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName); 
            var blobClient = containerClient.GetBlobClient(image.FileName);
            await using (var stream = image.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = image.ContentType }, cancellationToken:default);
            }
            return blobClient.Uri.ToString();
        }

    }
}
