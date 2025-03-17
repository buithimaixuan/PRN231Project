using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.DAOs
{
    public class FriendRequestDAO : SingletonBase<FriendRequestDAO>
    {
        // Lấy danh sách yêu cầu kết bạn mà tài khoản nhận được (ReceiverId = accountId, trạng thái Pending)
        public async Task<IEnumerable<FriendRequest>> GetFriendRequestReceivers(int accountId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender) // Bao gồm thông tin người gửi
                .Where(fr => fr.ReceiverId == accountId && fr.RequestStatus == "Pending")
                .ToListAsync();
        }

        // Lấy danh sách yêu cầu kết bạn mà tài khoản đã gửi (SenderId = accountId, trạng thái Pending)
        public async Task<IEnumerable<FriendRequest>> GetFriendRequestSenders(int accountId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Receiver) // Bao gồm thông tin người nhận
                .Where(fr => fr.SenderId == accountId && fr.RequestStatus == "Pending")
                .ToListAsync();
        }

        // Lấy danh sách bạn bè (RequestStatus = Accepted, tài khoản có thể là Sender hoặc Receiver)
        public async Task<IEnumerable<FriendRequest>> GetListFriends(int accountId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender) // Bao gồm thông tin người gửi
                .Include(fr => fr.Receiver) // Bao gồm thông tin người nhận
                .Where(fr => fr.RequestStatus == "Accepted" && (fr.SenderId == accountId || fr.ReceiverId == accountId))
                .ToListAsync();
        }
    }
}
