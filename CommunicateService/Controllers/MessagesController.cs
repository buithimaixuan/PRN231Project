using CommunicateService.Models;
using CommunicateService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommunicateService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        
        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
           
        }

        [HttpGet("{converId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllMessageByConverId(int converId)
        {
            IEnumerable<Message> list = await _messageService.GetAllMessageByConvId(converId);

            return Ok(list);
        }

        [HttpPost]
        public async Task<Message> AddMessage(Message message)
        {
            return await _messageService.AddMessage(message);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var mess = await _messageService.GetMessageByMesId(id);
            if (mess != null)
            {
                await _messageService.DeleteMessage(id);
            }
            return Ok();
        }


    }
}
