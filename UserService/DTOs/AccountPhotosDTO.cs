namespace UserService.DTOs
{
    public class AccountPhotosDTO
    {
        public int AccountId { get; set; }
        public int CountPhotos { get; set; }
        public IEnumerable<PostDTO.PostImage> Photos { get; set; }

        public AccountPhotosDTO(int accountId, int countPhotos, IEnumerable<PostDTO.PostImage> photos)
        {
            AccountId = accountId;
            CountPhotos = countPhotos;
            Photos = photos;
        }
    }
}
