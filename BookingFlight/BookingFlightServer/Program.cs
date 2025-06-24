using BookingFlightServer.Services.Implements;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Repositories;
using BookingFlightServer.Repositories.Implements;
using BookingFlightServer.Middlewares;
using BookingFlightServer.Repositories;
using BookingFlightServer.Proxies.Services;
using BookingFlightServer.Proxies.Services.Implements;
using BookingFlightServer.Entities;
using BookingFlightClient.Repositories;
using Library;

namespace BookingFlightServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			ConfigureServices(builder.Services);
			builder.Services.AddSingleton(sp =>
			{
				var config = sp.GetRequiredService<IConfiguration>();
				var connectionString = config["Redis:ConnectionString"]
					?? throw new ArgumentNullException();
				return new RedisHelper(connectionString);
			});
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options => {
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidAudience = builder.Configuration["Jwt:Audience"],
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
					RoleClaimType = ClaimTypes.Role,
					ClockSkew = TimeSpan.Zero
				};
			});
			builder.Services.AddAuthorization();

			var app = builder.Build();
			app.UseCors("AllowAll");
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMiddleware<CheckAccessApiMiddleware>();
			app.UseMiddleware<ExceptionHandlerMiddleware>();
			app.UseStaticFiles();

			app.UseHttpsRedirection();
			app.MapControllers();
			app.Run();
		}
		public static void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers()
				.ConfigureApiBehaviorOptions(
				options => options.SuppressModelStateInvalidFilter = true
				)
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
			services.AddDbContext<BookingFlightContext>(options => options.UseSqlServer("Name=ConnectionStrings:DBContext"));
			services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					builder =>
					{
						builder.AllowAnyOrigin()
							   .AllowAnyMethod()
							   .AllowAnyHeader();
					});
			});
			services.AddScoped(typeof(BookingFlightContext));
			services.AddTransient<IJwtService, JwtService>();
			services.AddTransient<IAccountService, AccountService>();
			services.AddTransient<IAccountRepository, AccountRepository>();
			services.AddTransient<IEmailService, EmailService>();
			services.AddTransient<IFlightService, FlightService>();
			services.AddTransient<IFlightRepository, FlightRepository>();
			services.AddTransient<IClassSeatRepository, ClassSeatRepository>();
			services.AddTransient<ICheckoutFlightService, CheckoutFlightService>();
			services.AddTransient<IClassSeatRepository, ClassSeatRepository>();
			services.AddTransient<IServiceRepository, ServiceRepository>();
			services.AddTransient<IVnPayService, VnPayService>();
			services.AddTransient<IPermissionApiRepository, PermissionApiRepository>();

		}
	}
}
