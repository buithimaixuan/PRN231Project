using CommunicateService.DAOs;
using CommunicateService.Models;

namespace CommunicateService.Repository.ConversationRepo
{
    public class ConversationRepository:IConversationRepository
    {
        private ConversationDAO conversationDAO;

        public ConversationRepository(ConversationDAO item)
        {
            conversationDAO = item;
        }
        public async Task<IEnumerable<Conversation>> GetAllConversationByAccId(int accId)
        {
            return await conversationDAO.GetAllConversationByAccId(accId);
        }
        public async Task<AccountConversation> GetAccConversationByAccOId(int conversId, int accId) => await conversationDAO.GetAccConversationByAccOId(conversId, accId);
        
    }
}

