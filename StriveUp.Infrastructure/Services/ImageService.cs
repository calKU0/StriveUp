using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StriveUp.Infrastructure.Data.Settings;
using StriveUp.Shared.Interfaces;
using System.Net;

namespace StriveUp.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "avatars"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.StatusCode != HttpStatusCode.OK || result.SecureUrl == null)
            {
                throw new ApplicationException($"Cloudinary upload failed: {result.Error?.Message ?? "Unknown error"}");
            }

            return result.SecureUrl.ToString();
        }
    }
}