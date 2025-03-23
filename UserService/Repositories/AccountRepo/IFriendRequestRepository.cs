using UserService.DTOs;
using UserService.Models;

namespace UserService.Repositories.AccountRepo
{
    public interface IFriendRequestRepository
    {
        Task<IEnumerable<FriendRequestDTO>> GetFriendRequestReceivers(int accountId);
        Task<IEnumerable<FriendRequestDTO>> GetFriendRequestSenders(int accountId);
        Task<IEnumerable<FriendRequestDTO>> GetListFriends(int accountId);
        Task<FriendRequest> SendFriendRequest(int senderId, int receiverId);
        Task UpdateFriendRequestStatus(int senderId, int receiverId, string status);
        Task Unfriend(int userId1, int userId2);
    }
}
