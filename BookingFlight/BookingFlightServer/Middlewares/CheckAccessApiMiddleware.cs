using BookingFlightClient.Repositories;
using BookingFlightServer.Entities;
using Library;
using System.Text.Json;

namespace BookingFlightServer.Middlewares
{
	public class CheckAccessApiMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<CheckAccessApiMiddleware> _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly RedisHelper _redisHelper;

		public CheckAccessApiMiddleware(RequestDelegate next, ILogger<CheckAccessApiMiddleware> logger, IServiceProvider serviceProvider, RedisHelper redisHelper)
		{
			_next = next;
			_logger = logger;
			_serviceProvider = serviceProvider;
			_redisHelper = redisHelper;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			using var scoped = _serviceProvider.CreateScope();
			var sessionId = context.Request.Headers["SessionId"];
			if (!sessionId.Any() || sessionId.Count == 0)
			{
				context.Response.StatusCode = 401;
				context.Response.ContentType = "application/json";
				return;
			}
			var _permissionApiRepository = scoped.ServiceProvider.GetRequiredService<IPermissionApiRepository>();
			var url = context.Request.Path;
			var ignorePaths = new[] { "/api/Home", "/api/login" };
			if (ignorePaths.Any(p => p.Equals(url)))
			{
				await _next(context);
				return;
			}
			var headerToken = context.Request.Headers["Authorization"];
			var role = !headerToken.Any() ? "Guest" : JwtDecoder.GetRoleInToken(headerToken.ToString().Substring("Bearer ".Length).Trim());
			string? redisValue = await _redisHelper.GetAsync($"{sessionId.ToString()}-{role}-Api");
			PermissionApi? permission = null;
			if (redisValue == null)
			{
				UserPermission<PermissionApi> userPermission = new()
				{
					SessionId = sessionId.ToString(),
					Role = role,
					Permissions = await _permissionApiRepository.GetPermissionByRole(role)
				};
				await _redisHelper.SetAsync($"{sessionId.ToString()}-{role}-Api", JsonSerializer.Serialize(userPermission));
				permission = await _permissionApiRepository.GetPermissionByRoleAndUrl(role, url);
			}
			else
			{
				UserPermission<PermissionApi> userPermission = JsonSerializer.Deserialize<UserPermission<PermissionApi>>(redisValue) ?? new();
				permission = userPermission.Permissions.FirstOrDefault(p => p.Url != null && p.Url.Equals(url));
			}
			if (permission == null)
			{
				context.Response.StatusCode = 401;
				context.Response.ContentType = "application/json";
				return;
			}
			await _next(context);
		}
	}
}
