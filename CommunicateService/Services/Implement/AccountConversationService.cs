using CommunicateService.DAOs;
using CommunicateService.Models;
using CommunicateService.Services.Interface;

namespace CommunicateService.Services.Implement
{
    public class AccountConversationService:IAccountConversationService
    {
        private readonly IAccountConversationService accountConversationService;
        public AccountConversationService(IAccountConversationService accountRepo)
        {
            accountConversationService = accountRepo;
        }
        public async Task<List<AccountConversation>> GetAllAccountConversation()
        {
            return await accountConversationService.GetAllAccountConversation();
        }
        public async Task<AccountConversation> GetByIdAccountConversation(int accId, int conversationId) => await accountConversationService.GetByIdAccountConversation(accId, conversationId);
        public async Task<AccountConversation> AddAccConversation(AccountConversation accountConversation)
        {
            return await accountConversationService.AddAccConversation(accountConversation);
        }
        public async Task<AccountConversation> UpdateAccConversation(AccountConversation item) => await accountConversationService.UpdateAccConversation(item);

        public async Task DeleteAccConversation(AccountConversation item) => await accountConversationService.DeleteAccConversation(item);
    }
}
