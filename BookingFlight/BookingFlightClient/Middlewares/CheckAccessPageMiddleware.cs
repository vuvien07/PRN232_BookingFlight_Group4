using BookingFlightClient.Entities;
using BookingFlightClient.Repositories;
using Library;
using System.IO;
using System.Text.Json;

namespace BookingFlightClient.Middlewares
{
	public class CheckAccessPageMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<CheckAccessPageMiddleware> _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly RedisHelper _redisHelper;

		public CheckAccessPageMiddleware(RequestDelegate next, ILogger<CheckAccessPageMiddleware> logger, IServiceProvider serviceProvider, RedisHelper redisHelper)
		{
			_next = next;
			_logger = logger;
			_serviceProvider = serviceProvider;
			_redisHelper = redisHelper;
		}

		public async Task Invoke(HttpContext context)
		{
			var sessionId = context.Session.GetString("sessionId");
			var sessionRole = context.Session.GetString("sessionRole");
			if (sessionId == null)
			{
				sessionId = Guid.NewGuid().ToString();
				context.Session.SetString("sessionId", sessionId);
			}
			if (sessionRole == null)
			{
				sessionRole = "Guest";
				context.Session.SetString("sessionRole", sessionRole);
			}
			using var scoped = _serviceProvider.CreateScope();
			var _permissionPageRepository = scoped.ServiceProvider.GetRequiredService<IPermissionPageRepository>();			var url = context.Request.Path;			var ignorePaths = new[] { 
				"/Unauthorized", "/Login", "/Register", "/Login/Register", 
				"/favicon.ico", "/api/AfterLogin", "/ForgotPassword", "/Register/VerifyEmail", "/Register/Success" 
			};
			var staticFileExtendsions = new[] { ".js", ".css", ".png", ".jpg", ".jpeg", ".svg", ".ico" };
			if(ignorePaths.Any(x => x.StartsWith(url.ToString())) || staticFileExtendsions.Contains(Path.GetExtension(url)))
			{
				await _next(context);
				return;
			}
			var role = sessionRole;
			string? redisValue = await _redisHelper.GetAsync($"{sessionId}-{role}-Page");
			PermissionPage? permission = null;
			if (redisValue == null)
			{
				UserPermission<PermissionPage> userPermission = new()
				{
					SessionId = sessionId,
					Role = role,
					Permissions = await _permissionPageRepository.GetListPermissionByRole(role)
				};
				await _redisHelper.SetAsync($"{sessionId}-{role}-Page", JsonSerializer.Serialize(userPermission));
				permission = await _permissionPageRepository.GetPermissionByRoleAndUrl(role, url);
			}
			else
			{
				UserPermission<PermissionPage> userPermission = JsonSerializer.Deserialize<UserPermission<PermissionPage>>(redisValue) ?? new();
				permission = userPermission.Permissions.FirstOrDefault(p => p.Url != null && p.Url.Equals(url));
			}
			if (permission == null)
			{
				context.Response.Redirect("/Unauthorized");
				return;
			}
			await _next(context);
		}
	}
}
