using CommunicateService.DAOs;
using CommunicateService.Models;

namespace CommunicateService.Repository.AccountConversationRepo
{
    public class AccountConversationRepository : IAccountConversationRepository
    {
        private AccountConversationDAO accountConversationDAO;

        public AccountConversationRepository(AccountConversationDAO item)
        {
            accountConversationDAO = item;
        }
        public async Task<List<AccountConversation>> GetAllAccountConversation()
        {
            return await accountConversationDAO.GetAllAccConversation();
        }

        public async Task<List<AccountConversation>> GetAllAccConversationByAccId(int accId)
        {
            return await accountConversationDAO.GetAllAccConversationByAccId(accId);
        }
        public async Task<AccountConversation> GetByIdAccountConversation(int accId, int conversationId) => await accountConversationDAO.GetByIdAccountConversation(accId, conversationId);
        public async Task<AccountConversation> AddAccConversation(AccountConversation accountConversation)
        {
            return await accountConversationDAO.AddAccountConversation(accountConversation);
        }
        public async Task<AccountConversation> UpdateAccConversation(AccountConversation item) => await accountConversationDAO.UpdateAccountConversation(item);

        public async Task DeleteAccConversation(AccountConversation item) => await accountConversationDAO.DeleteAccountConversation(item);
    }
}

