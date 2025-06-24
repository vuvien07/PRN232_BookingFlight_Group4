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
		}		public async Task Invoke(HttpContext context)
		{
			var sessionId = context.Session.GetString("sessionId");
			var sessionRole = context.Session.GetString("sessionRole");
			
			_logger.LogInformation($"CheckAccessPageMiddleware - Path: {context.Request.Path}, SessionId: {sessionId}, SessionRole: {sessionRole}");
			
			if (sessionId == null)
			{
				sessionId = Guid.NewGuid().ToString();
				context.Session.SetString("sessionId", sessionId);
				_logger.LogInformation($"Generated new sessionId: {sessionId}");
			}
			if (sessionRole == null)
			{
				sessionRole = "Guest";
				context.Session.SetString("sessionRole", sessionRole);
				_logger.LogInformation($"Set default sessionRole: {sessionRole}");
			}
			
			using var scoped = _serviceProvider.CreateScope();
			var _permissionPageRepository = scoped.ServiceProvider.GetRequiredService<IPermissionPageRepository>();
			var url = context.Request.Path;
			
			// TEMPORARY: Allow all access for authenticated users (for testing)
			if (sessionRole != "Guest")
			{
				_logger.LogInformation($"TEMPORARY: Allowing access for authenticated user with role: {sessionRole}");
				await _next(context);
				return;
			}
			
			// Expanded ignore paths for testing
			var ignorePaths = new[] { 
				"/Unauthorized", 
				"/Login", 
				"/favicon.ico", 
				"/api/AfterLogin",
				"/Home",
				"/Admin/Dashboard",
				"/Admin",
				"/Dashboard"
			};
			var staticFileExtendsions = new[] { ".js", ".css", ".png", ".jpg", ".jpeg", ".svg", ".ico" };
			
			if(ignorePaths.Any(x => url.ToString().StartsWith(x)) || staticFileExtendsions.Contains(Path.GetExtension(url)))
			{
				_logger.LogInformation($"Ignoring path: {url}");
				await _next(context);
				return;
			}
			
			var role = sessionRole;
			string? redisValue = await _redisHelper.GetAsync($"{sessionId}-{role}-Page");
			PermissionPage? permission = null;if (redisValue == null)
			{
				_logger.LogInformation($"No Redis cache found for {sessionId}-{role}-Page, creating new permissions");
				
				// Get all permissions for this role
				var rolePermissions = await _permissionPageRepository.GetListPermissionByRole(role);
				_logger.LogInformation($"Found {rolePermissions?.Count() ?? 0} permissions for role {role}");
				
				if (rolePermissions != null)
				{
					foreach (var perm in rolePermissions)
					{
						_logger.LogInformation($"Role {role} has permission for URL: {perm.Url}");
					}
				}
				
				UserPermission<PermissionPage> userPermission = new()
				{
					SessionId = sessionId,
					Role = role,
					Permissions = rolePermissions ?? new List<PermissionPage>()
				};
				await _redisHelper.SetAsync($"{sessionId}-{role}-Page", JsonSerializer.Serialize(userPermission));
				permission = await _permissionPageRepository.GetPermissionByRoleAndUrl(role, url);
				_logger.LogInformation($"Created permissions for role {role}, found permission for {url}: {permission != null}");
			}
			else
			{
				_logger.LogInformation($"Found Redis cache for {sessionId}-{role}-Page");
				UserPermission<PermissionPage> userPermission = JsonSerializer.Deserialize<UserPermission<PermissionPage>>(redisValue) ?? new();
				
				_logger.LogInformation($"Cached permissions count: {userPermission.Permissions?.Count() ?? 0}");
				if (userPermission.Permissions != null)
				{
					foreach (var perm in userPermission.Permissions)
					{
						_logger.LogInformation($"Cached permission URL: {perm.Url}");
					}
				}
				
				permission = userPermission.Permissions?.FirstOrDefault(p => p.Url != null && p.Url.Equals(url));
				_logger.LogInformation($"Found cached permission for {url}: {permission != null}");
			}
			
			// For testing - temporarily allow all for Admin role
			if (role == "Admin" || role == "admin" || role == "Administrator")
			{
				_logger.LogInformation($"Temporarily allowing all access for Admin role");
				await _next(context);
				return;
			}
			
			if (permission == null)
			{
				_logger.LogWarning($"No permission found for role {role} accessing {url}, redirecting to Unauthorized");
				_logger.LogWarning($"Available permissions in Redis key: {sessionId}-{role}-Page");
				context.Response.Redirect("/Unauthorized");
				return;
			}
			
			_logger.LogInformation($"Permission granted for role {role} accessing {url}");
			await _next(context);
		}
	}
}
