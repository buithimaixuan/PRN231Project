using UserService.DAOs;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Repositories.AccountRepo
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly FriendRequestDAO _friendRequestDAO;

        public FriendRequestRepository(FriendRequestDAO friendRequestDAO)
        {
            _friendRequestDAO = friendRequestDAO;
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetFriendRequestReceivers(int accountId)
        {
            return await _friendRequestDAO.GetFriendRequestReceivers(accountId);
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetFriendRequestSenders(int accountId)
        {
            return await _friendRequestDAO.GetFriendRequestSenders(accountId);
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetListFriends(int accountId)
        {
            return await _friendRequestDAO.GetListFriends(accountId);
        }
        public async Task<FriendRequest> SendFriendRequest(int senderId, int receiverId)
        {
            return await _friendRequestDAO.SendFriendRequest(senderId, receiverId);
        }

        public async Task UpdateFriendRequestStatus(int senderId, int receiverId, string status)
        {
            await _friendRequestDAO.UpdateFriendRequestStatus(senderId, receiverId, status);
        }
    }
}
