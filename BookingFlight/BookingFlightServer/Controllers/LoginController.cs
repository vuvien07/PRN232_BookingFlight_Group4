using BookingFlightServer.DTO;
using BookingFlightServer.Entiies;
using BookingFlightServer.Services;
using BookingFlightServer.Services.Implements;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IJwtService _jwtService;

		public LoginController(IAccountService accountService, IJwtService jwtService)
		{
			_accountService = accountService;
			_jwtService = jwtService;
		}

		[HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(UtilHelper.GetModelStateErrors(ModelState));
			}
			Account? account = await _accountService.findByUsernameAndPassword(loginDTO.Username, loginDTO.Password);
			if (account == null) return BadRequest(new { message = "Username or password is incorrect" });
			string token = _jwtService.CreateJwtToken(Mapper.Map<Account, AccountDTO>(account) ?? new AccountDTO());
			string refreshToken = _jwtService.CreateRefreshToken();
			DateTime dateTime = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtService.GetTokenExpirationTime()));
			account.RefreshToken = refreshToken;
			account.RefreshTokenExpiryTime = dateTime;
			await _accountService.UpdateAccountAsync(account);
			return Ok(new { token = token, refreshToken = refreshToken });
		}
    }
}
