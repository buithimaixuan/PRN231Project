using CommunicateService.Models;
using CommunicateService.Repository.ConversationRepo;
using CommunicateService.Services.Interface;

namespace CommunicateService.Services.Implement
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;

        public ConversationService(IConversationRepository conversationRepository)
        {
            _conversationRepository = conversationRepository;
        }

        public async Task<IEnumerable<Conversation>> GetAllConversationByAccId(int accId)
        {
            return await _conversationRepository.GetAllConversationByAccId(accId);
        }
        public async Task<AccountConversation> GetAccConversationByAccOId(int conversId, int accId) => await _conversationRepository.GetAccConversationByAccOId(conversId, accId);


    }
}
