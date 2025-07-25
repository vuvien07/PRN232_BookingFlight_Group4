using BookingFlightServer.DTO.Chat;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(UtilHelper.GetModelStateErrors(ModelState));
                }

                var message = await _chatService.SendMessageAsync(request);
                return Ok(new { success = true, data = message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory([FromQuery] ChatHistoryRequest request)
        {
            try
            {
                var messages = await _chatService.GetChatHistoryAsync(request);
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("customer-conversations/{customerId}")]
        public async Task<IActionResult> GetCustomerConversations(int customerId)
        {
            try
            {
                var conversations = await _chatService.GetCustomerConversationsAsync(customerId);
                return Ok(new { success = true, data = conversations });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("supporter-conversations/{supporterId}")]
        public async Task<IActionResult> GetSupporterConversations(int supporterId)
        {
            try
            {
                var conversations = await _chatService.GetSupporterConversationsAsync(supporterId);
                return Ok(new { success = true, data = conversations });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("mark-read/{messageId}")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            try
            {
                var result = await _chatService.MarkMessageAsReadAsync(messageId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount([FromQuery] int customerId, [FromQuery] int? supporterId)
        {
            try
            {
                var count = await _chatService.GetUnreadMessageCountAsync(customerId, supporterId);
                return Ok(new { success = true, unreadCount = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
