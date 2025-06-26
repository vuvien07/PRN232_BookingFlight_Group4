using BookingFlightClient.Entities;
using BookingFlightClient.Middlewares;
using BookingFlightClient.Repositories;
using BookingFlightClient.Repositories.Implements;
using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddDbContext<BookingFlightContext>(options => options.UseSqlServer("Name=ConnectionStrings:DBContext"));
			builder.Services.AddScoped(typeof(BookingFlightContext));
			builder.Services.AddTransient<IPermissionPageRepository, PermissionPageRepository>();
			builder.Services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			builder.Services.AddSingleton(sp =>
			{
				var config = sp.GetRequiredService<IConfiguration>();
				var connectionString = config["Redis:ConnectionString"]
					?? throw new ArgumentNullException("Redis:ConnectionString");
				return new RedisHelper(connectionString);
			});
			builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation() ;
			var app = builder.Build();
			app.UseSession();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseMiddleware<CheckAccessPageMiddleware>();
			app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}");
			app.Run();
		}
	}
}
