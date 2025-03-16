using CommunicateService.DAOs;
using CommunicateService.Models;
using CommunicateService.Repository.AccountConversationRepo;
using CommunicateService.Services.Interface;

namespace CommunicateService.Services.Implement
{
    public class AccountConversationService : IAccountConversationService
    {
        private readonly IAccountConversationRepository accountConversationService;
        public AccountConversationService(IAccountConversationRepository accountRepo)
        {
            accountConversationService = accountRepo;
        }
        public async Task<List<AccountConversation>> GetAllAccountConversation()
        {
            return await accountConversationService.GetAllAccountConversation();
        }

        public async Task<List<AccountConversation>> GetAllAccConversationByAccId(int accId)
        {
            return await accountConversationService.GetAllAccConversationByAccId(accId);
        }
        public async Task<AccountConversation> GetAccConverByAccIdAndConverId(int accId, int conversationId) => await accountConversationService.GetByIdAccountConversation(accId, conversationId);
        public async Task<AccountConversation> AddAccConversation(AccountConversation accountConversation)
        {
            return await accountConversationService.AddAccConversation(accountConversation);
        }
        public async Task<AccountConversation> UpdateAccConversation(AccountConversation item) => await accountConversationService.UpdateAccConversation(item);

        public async Task DeleteAccConversation(AccountConversation item) => await accountConversationService.DeleteAccConversation(item);
    }
}
