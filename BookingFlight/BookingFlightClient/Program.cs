using BookingFlightClient.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BookingFlightClient.Services;

namespace BookingFlightClient
{
	public class Program
	{
		public static void Main(string[] args)
		{
		var builder = WebApplication.CreateBuilder(args);
		
		// Configure HttpClient with SSL bypass for development
		builder.Services.AddHttpClient();
		builder.Services.AddHttpClient("IgnoreSSL", client =>
		{
			// Configure client as needed
		}).ConfigurePrimaryHttpMessageHandler(() =>
		{
			return new HttpClientHandler()
			{
				ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
			};
		});
		
		builder.Services.AddSession(options =>
		{
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});			// Register services
			builder.Services.AddScoped<INewsService, NewsService>();
			builder.Services.AddScoped<IComplaintService, ComplaintService>();
			
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
            builder.Services.AddControllersWithViews();
			var app = builder.Build();
			app.UseSession();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseMiddleware<GetRequireRoleMiddleware>();
			app.UseMiddleware<JwtSessionMiddleware>();
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.Run();
		}
	}
}
