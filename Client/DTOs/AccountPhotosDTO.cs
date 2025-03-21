namespace Client.DTOs
{
    public class AccountPhotosDTO
    {
        public int AccountId { get; set; }
        public int CountPhotos { get; set; }
        public IEnumerable<PostImage> Photos { get; set; }

        public class PostImage
        {
            public int ImageId { get; set; }
            public string ImageUrl { get; set; }
            public bool? IsDeleted { get; set; }
        }
    }
}
