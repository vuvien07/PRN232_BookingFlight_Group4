using System.Text.Json;

namespace BookingFlightServer.Middlewares
{
	public class ExceptionHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlerMiddleware> _logger;
		public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred");
				context.Response.StatusCode = 500;
				context.Response.ContentType = "application/json";
				var errorResponse = new
				{
					status = 500,
					message = "Có lỗi xảy ra: " + ex.Message
				};
				var json = JsonSerializer.Serialize(errorResponse);
				await context.Response.WriteAsync(json);
			}
		}
	}
}
