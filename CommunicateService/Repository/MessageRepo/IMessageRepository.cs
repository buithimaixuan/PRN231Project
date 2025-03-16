using CommunicateService.Models;

namespace CommunicateService.Repository.MessageRepo
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetAllMessageByConvId(int accId);
        Task<Message> AddMessage(Message message);
        Task<Message> GetMessageByMesId(int id);
        Task DeleteMessage(int id);
    }
}
