using CommunicateService.Models;

namespace CommunicateService.Services.Interface
{
    public interface IAccountConversationService
    {
        Task<List<AccountConversation>> GetAllAccountConversation();
        Task<List<AccountConversation>> GetAllAccConversationByAccId(int accid);
        Task<AccountConversation> GetAccConverByAccIdAndConverId(int accId, int conversationId);
        Task DeleteAccConversation(AccountConversation accountConversation);
        Task<AccountConversation> UpdateAccConversation(AccountConversation conversation);
        Task<AccountConversation> AddAccConversation(AccountConversation conversation);
    }
}