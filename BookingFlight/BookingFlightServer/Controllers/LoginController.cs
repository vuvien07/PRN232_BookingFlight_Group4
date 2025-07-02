using Azure.Core;
using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Shared;
using BookingFlightServer.Entities;
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
			var accessTokenCookie = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtService.GetTokenExpirationTime())),
				Path = "/"
			};

			var refreshTokenCookie = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.Now.AddDays(7),
				Path = "/"
			};

			Response.Cookies.Append("X-Access-Token", token, accessTokenCookie);
			Response.Cookies.Append("X-Refresh-Token", refreshToken, refreshTokenCookie);
			return Ok(new { message = "Login success" });
		}

		[HttpGet("refresh-token")]
		public async Task<IActionResult> RefreshToken()
		{
			if (!Request.Cookies.TryGetValue("X-Refresh-Token", out var token))
			{
				return Unauthorized(new { message = "Missing refresh token" });
			}
			Account? account = await _accountService.FindByRefreshTokenAsync(token);
			if(account == null || account.RefreshTokenExpiryTime < DateTime.Now)
			{
				return Unauthorized(new { message = "Invalid refresh token" });
			}
			string newToken = _jwtService.CreateJwtToken(Mapper.Map<Account, AccountDTO>(account) ?? new AccountDTO());
			DateTime dateTime = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtService.GetTokenExpirationTime()));
			await _accountService.UpdateAccountAsync(account);
			var newAccessTokenCookie = new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtService.GetTokenExpirationTime())),
				Path = "/"
			};
			Response.Cookies.Append("X-Access-Token", newToken, newAccessTokenCookie);
			return Ok("Refresh token success");
		}
    }
}
