using CommunicateService.Models;

namespace CommunicateService.Services.Interface
{
    public interface IConversationService
    {
        Task<IEnumerable<Conversation>> GetAllConversationByAccId(int accId);
        Task<AccountConversation> GetAccConversationByAccOId(int converId, int accId);
    }
}