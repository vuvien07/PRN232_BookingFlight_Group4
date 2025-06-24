using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BookingFlightClient.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AfterLoginController : ControllerBase
    {
        [HttpGet]
        public IActionResult SetRoleInSession([FromQuery] string role)
        {
            try
            {
                // Update session role
                HttpContext.Session.SetString("sessionRole", role);
                
                // Also try to get sessionId and ensure it's set
                var sessionId = HttpContext.Session.GetString("sessionId");
                if (string.IsNullOrEmpty(sessionId))
                {
                    sessionId = Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("sessionId", sessionId);
                }
                
                // Force session to be committed
                HttpContext.Session.CommitAsync().Wait();
                
                Console.WriteLine($"Session updated - SessionId: {sessionId}, Role: {role}");
                
                return Ok(new { 
                    success = true, 
                    sessionId = sessionId, 
                    role = role,
                    message = "Session updated successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating session: {ex.Message}");
                return BadRequest(new { 
                    success = false, 
                    message = ex.Message 
                });
            }
        }
    }
}
