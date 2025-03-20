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
            // Kiểm tra xem đã có yêu cầu từ senderId đến receiverId chưa
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);

            if (existingRequest != null)
            {
                throw new Exception("A friend request already exists between these users.");
            }

            // Kiểm tra xem đã có yêu cầu ngược lại từ receiverId đến senderId chưa
            var reverseRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == receiverId && fr.ReceiverId == senderId && fr.RequestStatus == "pending");

            if (reverseRequest != null)
            {
                // Nếu có yêu cầu ngược lại, tự động chấp nhận yêu cầu
                reverseRequest.RequestStatus = "accepted";
                reverseRequest.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                await _context.SaveChangesAsync();
                return reverseRequest;
            }

            // Kiểm tra xem receiver có tồn tại không
            var receiver = await _context.Accounts.FindAsync(receiverId);
            if (receiver == null)
            {
                throw new Exception("Receiver does not exist or has been deleted.");
            }

            // Tạo yêu cầu kết bạn mới
            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                RequestStatus = "pending",
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
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

            if (status == "accepted")
            {
                friendRequest.RequestStatus = status;
                friendRequest.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            }
            else if (status == "rejected")
            {
                _context.FriendRequests.Remove(friendRequest);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Unfriend(int userId1, int userId2)
        {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.RequestStatus == "accepted" &&
                                          ((fr.SenderId == userId1 && fr.ReceiverId == userId2) ||
                                           (fr.SenderId == userId2 && fr.ReceiverId == userId1)));

            if (friendRequest == null)
            {
                throw new Exception($"Friend relationship not found between user {userId1} and user {userId2}.");
            }

            _context.FriendRequests.Remove(friendRequest);
            var changes = await _context.SaveChangesAsync();
            if (changes == 0)
            {
                throw new Exception("Failed to unfriend: No changes were saved to the database.");
            }
        }
    }
}
