using CommunicateService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunicateService.DAOs
{
    public class AccountConversationDAO:SingletonBase<AccountConversationDAO>
    {
        private readonly MicroserviceCommunicateDbContext _context;

        // Constructor sử dụng SingletonBase để lấy DbContext
        public AccountConversationDAO()
        {
            _context = SingletonBase<MicroserviceCommunicateDbContext>.Instance;
        }
        // Lấy tất cả AccountConversation (Async)
        public async Task<List<AccountConversation>> GetAllAccConversation()
        {
            return await _context.AccountConversations.ToListAsync();
        }

        //Lấy AccountConversation theo ID (Async)
        public async Task<AccountConversation> GetByIdAccountConversation(int accountId, int conversationId)
        {
            return await _context.AccountConversations
                                 .FirstOrDefaultAsync(ac => ac.AccountId == accountId && ac.ConversationId == conversationId);
        }

        // Thêm mới 
        public async Task<AccountConversation> AddAccountConversation(AccountConversation entity)
        {
            await _context.AccountConversations.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // (Async)
        public async Task DeleteAccountConversation(AccountConversation entity)
        {
            _context.AccountConversations.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // Cập nhật (Async)
        public async Task<AccountConversation> UpdateAccountConversation(AccountConversation entity)
        {
            _context.AccountConversations.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
