using CommunicateService.Models;

namespace CommunicateService.Repository.MessageRepo
{
    public interface IMessageRepoository
    {
        Task<IEnumerable<Message>> GetAllMessageByConvId(int accId);
        Task<Message> AddMessage(Message message);
        Task DeleteMessage(int id);
    }
}
