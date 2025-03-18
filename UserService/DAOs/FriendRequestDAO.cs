using Microsoft.EntityFrameworkCore;
using UserService.DTOs;
using UserService.Models;

namespace UserService.DAOs
{
    public class FriendRequestDAO : SingletonBase<FriendRequestDAO>
    {
        public async Task<IEnumerable<FriendRequestDTO>> GetFriendRequestReceivers(int accountId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.ReceiverId == accountId && fr.RequestStatus == "pending")
                .Select(fr => new FriendRequestDTO
                {
                    RequestId = fr.RequestId,
                    SenderId = fr.SenderId,
                    ReceiverId = fr.ReceiverId,
                    RequestStatus = fr.RequestStatus,
                    CreatedAt = fr.CreatedAt,
                    UpdatedAt = fr.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetFriendRequestSenders(int accountId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.SenderId == accountId && fr.RequestStatus == "pending")
                .Select(fr => new FriendRequestDTO
                {
                    RequestId = fr.RequestId,
                    SenderId = fr.SenderId,
                    ReceiverId = fr.ReceiverId,
                    RequestStatus = fr.RequestStatus,
                    CreatedAt = fr.CreatedAt,
                    UpdatedAt = fr.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetListFriends(int accountId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.RequestStatus == "accepted" && (fr.SenderId == accountId || fr.ReceiverId == accountId))
                .Select(fr => new FriendRequestDTO
                {
                    RequestId = fr.RequestId,
                    SenderId = fr.SenderId,
                    ReceiverId = fr.ReceiverId,
                    RequestStatus = fr.RequestStatus,
                    CreatedAt = fr.CreatedAt,
                    UpdatedAt = fr.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<FriendRequest> SendFriendRequest(int senderId, int receiverId)
        {
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if (existingRequest != null)
            {
                throw new Exception("A friend request already exists between these users.");
            }

            var receiver = await _context.Accounts.FindAsync(receiverId);
            if (receiver == null)
            {
                throw new Exception("Receiver does not exist or has been deleted.");
            }

            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                RequestStatus = "pending",
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                UpdatedAt = null
            };

            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();
            return friendRequest;
        }

        public async Task UpdateFriendRequestStatus(int senderId, int receiverId, string status)
        {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId && fr.RequestStatus == "pending");

            if (friendRequest == null)
            {
                throw new Exception("Friend request not found or already processed.");
            }

            if (status != "accepted" && status != "rejected")
            {
                throw new ArgumentException("Status must be either 'accepted' or 'rejected'.");
            }

            friendRequest.RequestStatus = status;
            friendRequest.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            await _context.SaveChangesAsync();
        }
    }
}
