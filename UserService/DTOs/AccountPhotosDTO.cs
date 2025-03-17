namespace UserService.DTOs
{
    public class AccountPhotosDTO
    {
        public int AccountId { get; set; }
        public IEnumerable<PostDTO.PostImage> Photos { get; set; }

        public AccountPhotosDTO(int accountId, IEnumerable<PostDTO.PostImage> photos)
        {
            AccountId = accountId;
            Photos = photos;
        }
    }
}
