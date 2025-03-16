using CommunicateService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunicateService.DAOs
{
    public class ConversationDAO : SingletonBase<ConversationDAO>
    {
        //private readonly MicroserviceCommunicateDbContext _context;

        //public ConversationDAO(MicroserviceCommunicateDbContext context)
        //{
        //    _context = context;
        //}


        public async Task<IEnumerable<Conversation>> GetAllConversationByAccId(int accId)
        {
            return await (from c in _context.Conversations
                          join ac in _context.AccountConversations on c.ConversationId equals ac.ConversationId
                          where ac.AccountId == accId
                                && ac.IsOut == false
                                && c.IsDeleted == false
                          select c).ToListAsync();
        }

        public async Task<AccountConversation> GetAccConversationByAccOId(int convId, int accId)
        {
            var item = await _context.AccountConversations.FirstOrDefaultAsync(c => c.ConversationId == convId && c.AccountId == accId);
            if (item == null) return null;
            return item;
        }


    }
}
