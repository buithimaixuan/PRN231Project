using CommunicateService.Models;

namespace CommunicateService.Repository.AccountConversationRepo
{
    public interface IAccountConversationRepository
    {
        Task<List<AccountConversation>> GetAllAccountConversation();
        Task<List<AccountConversation>> GetAllAccConversationByAccId(int accid);
        Task<AccountConversation> GetByIdAccountConversation(int accId, int conversationId);
        Task DeleteAccConversation(AccountConversation accountConversation);
        Task<AccountConversation> UpdateAccConversation(AccountConversation conversation);
        Task<AccountConversation> AddAccConversation(AccountConversation conversation);
    }
}
