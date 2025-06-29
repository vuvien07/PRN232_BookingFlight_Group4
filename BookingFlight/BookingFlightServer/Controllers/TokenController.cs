using BookingFlightServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly IJwtService _jwtService;

		public TokenController(IJwtService jwtService)
		{
			_jwtService = jwtService;
		}

		[HttpGet("get")]
		[Authorize]
		public IActionResult GetUsernameAndRoleInToken()
		{
			string? token = Request.Cookies["X-Access-Token"];
			if (string.IsNullOrEmpty(token))
			{
				return Unauthorized("Missing access token");
			}
			var decodedToken = _jwtService.DecodeJwtToken(token);
			return Ok(decodedToken);
		}
	}
}
