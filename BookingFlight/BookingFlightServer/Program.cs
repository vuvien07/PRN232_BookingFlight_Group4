using BookingFlightServer.Data;
using BookingFlightServer.Middlewares;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace BookingFlightServer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingFlightServer", Version = "v1" });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
            });
            ConfigureServices(builder.Services, builder.Configuration);
			builder.Services.AddAllServices(typeof(Program).Assembly);
			builder.Services.AddAllRepositories(typeof(Program).Assembly);
			var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            }
            app.UseCors("AllowFrontEndClient");
			app.UseMiddleware<CookieToHeaderMiddleware>();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseMiddleware<ExceptionHandlerMiddleware>();
			app.UseStaticFiles();

			app.UseHttpsRedirection();
			app.MapControllers();
			app.Run();
		}
		public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddControllers()
				.ConfigureApiBehaviorOptions(
				options => options.SuppressModelStateInvalidFilter = true
				)
				.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
			services.AddDbContext<BookingFlightContext>(options => options.UseSqlServer("Name=ConnectionStrings:DBContext"));
			services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontEndClient",
					builder =>
					{
						builder.WithOrigins("http://localhost:5001")
						.AllowCredentials()
							   .AllowAnyMethod()
							   .AllowAnyHeader();
					});
			});
			services.AddScoped(typeof(BookingFlightContext));

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidAudience = configuration["Jwt:Audience"],
					ValidIssuer = configuration["Jwt:Issuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
					RoleClaimType = ClaimTypes.Role,
					ClockSkew = TimeSpan.Zero
				};
			});
			services.AddAuthorization();

		}
	}
}
