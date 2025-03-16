using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace PostService.Config
{
    public class CloudinaryConfig
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryConfig(IConfiguration configuration)
        {
            var cloudName = configuration.GetValue<string>("Cloudinary:CloudName");
            var apiKey = configuration.GetValue<string>("Cloudinary:ApiKey");
            var apiSecret = configuration.GetValue<string>("Cloudinary:ApiSecret");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<(string ImageUrl, string PublicId)> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ.");

            using var stream = file.OpenReadStream(); // Chuyển đổi IFormFile thành Stream
            var publicId = $"Home/PRN231/{Guid.NewGuid()}"; // Tạo Public ID

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return (uploadResult.SecureUrl.AbsoluteUri, uploadResult.PublicId);
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
            return deleteResult.Result == "ok";
        }
    }
}
