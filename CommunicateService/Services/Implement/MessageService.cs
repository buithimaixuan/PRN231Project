using CommunicateService.DAOs;
using CommunicateService.Models;
using CommunicateService.Repository.MessageRepo;
using CommunicateService.Services.Interface;

namespace CommunicateService.Services.Implement
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageService;
        public MessageService(IMessageRepository accountRepo)
        {
            messageService = accountRepo;
        }

        public async Task<IEnumerable<Message>> GetAllMessageByConvId(int accId)
        {
            return await messageService.GetAllMessageByConvId(accId);
        }
        public async Task DeleteMessage(int id) => await messageService.DeleteMessage(id);

        public async Task<Message> AddMessage(Message item) => await messageService.AddMessage(item);
        public async Task<Message> GetMessageByMesId(int id) => await messageService.GetMessageByMesId(id);
    }
}
