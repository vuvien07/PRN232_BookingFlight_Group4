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
			var url = context.Request.Path;
			
			// Debug logging
			Console.WriteLine($"API Middleware - URL: {url}, SessionId: {sessionId}");
			
			if (!sessionId.Any() || sessionId.Count == 0)
			{				// For register, forgot password, and email verification endpoints, we don't need SessionId
				var ignorePaths = new[] { "/api/Home", "/api/login", "/api/Register" };
				var ignorePathsStartsWith = new[] { 
					"/api/Register/check-", 
					"/api/Register/test",
					"/api/ForgotPassword/",
					"/api/EmailVerification/"
				};
				
				Console.WriteLine($"No SessionId - checking if URL should be ignored: {url}");
				
				if (ignorePaths.Any(p => p.Equals(url, StringComparison.OrdinalIgnoreCase)) || 
				    ignorePathsStartsWith.Any(p => url.ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase)))
				{
					Console.WriteLine($"URL {url} is in ignore list - allowing access");
					await _next(context);
					return;
				}
				
				Console.WriteLine($"URL {url} not in ignore list - returning 401");
				context.Response.StatusCode = 401;
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync("{\"message\":\"Unauthorized - No SessionId\"}");
				return;
			}
					var _permissionApiRepository = scoped.ServiceProvider.GetRequiredService<IPermissionApiRepository>();
			var ignorePaths2 = new[] { "/api/Home", "/api/login", "/api/Register" };
			var ignorePathsStartsWith2 = new[] { 
				"/api/Register/check-", 
				"/api/Register/test",
				"/api/ForgotPassword/",
				"/api/EmailVerification/"
			};
			if (ignorePaths2.Any(p => p.Equals(url, StringComparison.OrdinalIgnoreCase)) || 
			    ignorePathsStartsWith2.Any(p => url.ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase)))
			{
				Console.WriteLine($"URL {url} is in ignore list - allowing access");
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
			}			if (permission == null)
			{
				Console.WriteLine($"No permission found for URL {url} and role {role} - returning 401");
				context.Response.StatusCode = 401;
				context.Response.ContentType = "application/json";
				await context.Response.WriteAsync("{\"message\":\"Unauthorized - No permission\"}");
				return;
			}
			Console.WriteLine($"Permission found for URL {url} - allowing access");
			await _next(context);
		}
	}
}
