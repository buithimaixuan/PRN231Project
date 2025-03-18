namespace UserService.DTOs
{
    public class FriendRequestDTO
    {
        public int RequestId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string RequestStatus { get; set; } = null!;
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
