using CommunicateService.Models;
using CommunicateService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunicateService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("{accId}")]
        public async Task<ActionResult<IEnumerable<Conversation>>> GetAllConversationByAccId(int accId)
        {
            IEnumerable<Conversation> list = await _conversationService.GetAllConversationByAccId(accId);
            return Ok(list);
        }

        [HttpGet("{converId},{accId}")]
        public async Task<ActionResult<AccountConversation>> GetAccConversationByAccOId(int converId, int accId)
        {
            var accConversation = await _conversationService.GetAccConversationByAccOId(converId, accId);
            return Ok(accConversation);
        }
    }
}
