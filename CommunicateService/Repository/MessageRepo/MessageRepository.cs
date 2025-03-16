using CommunicateService.DAOs;
using CommunicateService.Models;

namespace CommunicateService.Repository.MessageRepo
{
    public class MessageRepository : IMessageRepository
    {
        private MessageDAO messageDAO;

        public MessageRepository(MessageDAO item)
        {
            messageDAO = item;
        }
        public async Task<IEnumerable<Message>> GetAllMessageByConvId(int accId)
        {
            return await messageDAO.GetAllMessageByConvId(accId);
        }
        public async Task DeleteMessage(int id) => await messageDAO.DeleteMessage(id);

        public async Task<Message> AddMessage(Message item) => await messageDAO.AddMessage(item);
        public async Task<Message> GetMessageByMesId(int id) => await messageDAO.GetMessageByMesId(id);
    }
}

