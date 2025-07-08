using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace net.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["CloudinarySettings:CloudName"],
                config["CloudinarySettings:ApiKey"],
                config["CloudinarySettings:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file.Length <= 0)
                throw new ArgumentException("File is empty.");

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = Path.GetFileNameWithoutExtension(file.FileName),
                Overwrite = true,
                Folder = "uploads"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return result.SecureUrl.AbsoluteUri;
            }

            throw new Exception("Image upload failed: " + result.Error?.Message);
        }

        public async Task<bool> DeleteImageAsync(string filename)
        {
            var publicId = $"uploads/{Path.GetFileNameWithoutExtension(filename)}";

            var deleteParams = new DeletionParams(publicId)
            {
                Invalidate = true
            };
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok" || result.Result == "not_found";
        }
    }
}
