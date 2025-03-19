namespace Client.DTOs
{
    public class ImageUploadResponseDTO
    {
        public string ImageUrl { get; set; }
        public string PublicId { get; set; }

        public ImageUploadResponseDTO(string imageUrl, string publicId)
        {
            ImageUrl = imageUrl;
            PublicId = publicId;
        }

    }
}
