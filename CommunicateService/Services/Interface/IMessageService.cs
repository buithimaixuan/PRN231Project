using CommunicateService.Models;

namespace CommunicateService.Services.Interface
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAllMessageByConvId(int accId);
        Task<Message> AddMessage(Message message);
        Task<Message> GetMessageByMesId(int id);
        Task DeleteMessage(int id);
    }
}
