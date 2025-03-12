using CommunicateService.Models;

namespace CommunicateService.Repository.ConversationRepo
{
    public interface IConversationRepository
    {
        Task<IEnumerable<Conversation>> GetAllConversationByAccId(int accId);
        Task<AccountConversation> GetAccConversationByAccOId(int converId, int accId);
    }
}
