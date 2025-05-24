using Microsoft.AspNetCore.Http;

namespace StriveUp.Shared.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}