using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookingFlightClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddHttpClient();
			builder.Services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
			 options =>
				{
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							var token = context.Request.Cookies["X-Access-Token"];
							if (!string.IsNullOrEmpty(token))
							{
								context.Token = token;
							}
							return Task.CompletedTask;
						},
						OnChallenge = context =>
					   {
						   context.HandleResponse();
						   context.Response.Redirect("/Unauthorized?returnUrl=" + context.HttpContext.Request.Path);
						   return Task.CompletedTask;
					   },
						OnForbidden = context =>
						{
							context.Response.Redirect("/Unauthorized?returnUrl=" + context.HttpContext.Request.Path);
							return Task.CompletedTask;
						}
					};
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(
			   Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
					};

				});
            builder.Services.AddHttpClient();
            builder.Services.AddControllersWithViews();
			var app = builder.Build();
			app.UseSession();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}");
			app.Run();
		}
	}
}
