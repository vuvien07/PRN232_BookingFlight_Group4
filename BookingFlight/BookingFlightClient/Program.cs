using Microsoft.EntityFrameworkCore;

namespace BookingFlightClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			
			builder.Services.AddControllersWithViews();
			var app = builder.Build();
			app.UseSession();
			app.UseStaticFiles();
			app.UseRouting();
			app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}");
			app.Run();
		}
	}
}
