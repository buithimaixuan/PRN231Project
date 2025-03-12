using CommunicateService.Models;

namespace CommunicateService.Services.Interface
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessageByConvId(int accId);
        Task<Message> AddMessage(Message message);
        Task DeleteMessage(int id);
    }
}
