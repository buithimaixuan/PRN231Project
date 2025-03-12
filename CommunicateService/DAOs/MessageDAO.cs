using CommunicateService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunicateService.DAOs
{
    public class MessageDAO:SingletonBase<MessageDAO>
    {
        private readonly MicroserviceCommunicateDbContext _context;

        // Constructor sử dụng SingletonBase để lấy DbContext
        public MessageDAO()
        {
            _context = SingletonBase<MicroserviceCommunicateDbContext>.Instance;
        }

        public async Task<IEnumerable<Message>> GetAllMessageByConvId(int conv)
        {
            return await _context.Messages.Where(m => m.ConversationId == conv).ToListAsync();
        }

        public async Task<Message> AddMessage(Message item)
        {
            _context.Messages.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task DeleteMessage(int messageId)
        {
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.MessageId == messageId);
            if (message != null)
            {
                message.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
