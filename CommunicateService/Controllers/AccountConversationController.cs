using CommunicateService.Models;
using CommunicateService.Services.Implement;
using CommunicateService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunicateService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountConversationController : ControllerBase
    {
        private readonly IAccountConversationService _conversationService;
        public AccountConversationController(IAccountConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountConversation>>> GetAllAccountConversation() { 
            return await _conversationService.GetAllAccountConversation();
        }
        [HttpGet("{accId}")]
        public async Task<ActionResult<List<AccountConversation>>> GetAllAccConversationByAccId(int accId)
        {
            return await _conversationService.GetAllAccConversationByAccId(accId);
        }

        [HttpGet("{converId},{accId}")]
        public async Task<ActionResult<AccountConversation>> GetAccConverByAccIdAndConverId(int converId, int accId)
        {
            var accConversation = await _conversationService.GetAccConverByAccIdAndConverId(converId, accId);
            return Ok(accConversation);
        }

        
    }
}
