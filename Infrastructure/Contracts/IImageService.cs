using Microsoft.AspNetCore.Http;

namespace Infrastructure.Contracts
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile image);
    }
}
