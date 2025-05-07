using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using StriveUp.Shared.Interfaces;
using StriveUp.Infrastructure.Data.Settings;

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
