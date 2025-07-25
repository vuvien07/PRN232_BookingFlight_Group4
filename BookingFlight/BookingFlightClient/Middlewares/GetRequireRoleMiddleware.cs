using Microsoft.AspNetCore.Authorization;

namespace BookingFlightClient.Middlewares
{
	public class GetRequireRoleMiddleware
	{
		private readonly RequestDelegate _next;

		public GetRequireRoleMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			var endpoint = context.GetEndpoint();
			if (endpoint != null)
			{
				var authorizeMetadata = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
				if (authorizeMetadata != null)
				{
					var roles = authorizeMetadata.Roles;
					if (!string.IsNullOrEmpty(roles))
					{
						context.Response.Cookies.Append(
							"X-RequiredRoles",
							roles.ToString(),
							new CookieOptions
							{
								HttpOnly = true,
								Secure = true,
								SameSite = SameSiteMode.Strict
							}
						);

						Console.WriteLine($"RequiredRoles from attribute: {roles}");
					}
				}
			}
			await _next(context);
		}
	}
}
