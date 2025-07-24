using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookingFlightClient.Middlewares
{
    public class JwtSessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtSessionMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get JWT token from cookie
            var token = context.Request.Cookies["X-Access-Token"];
            
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    
                    // Validate token
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty)),
                        ClockSkew = TimeSpan.Zero
                    };

                    ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    
                    // Extract claims from JWT
                    var jwtToken = validatedToken as JwtSecurityToken;
                    if (jwtToken != null)
                    {
                        // Debug: Print all claims
                        Console.WriteLine("=== JWT Claims Debug ===");
                        foreach (var claim in jwtToken.Claims)
                        {
                            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
                        }
                        Console.WriteLine("========================");

                        // Get role from JWT claims - try multiple possible claim types
                        var roleClaimValue = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                        var accountIdClaimValue = jwtToken.Claims.FirstOrDefault(x => x.Type == "AccountId")?.Value;
                        
                        Console.WriteLine($"Role found: {roleClaimValue}");
                        Console.WriteLine($"AccountId found: {accountIdClaimValue}");
                        
                        if (!string.IsNullOrEmpty(accountIdClaimValue) && int.TryParse(accountIdClaimValue, out int accountId))
                        {
                            // Based on role, set appropriate session
                            if (!string.IsNullOrEmpty(roleClaimValue))
                            {
                                Console.WriteLine($"Setting session for role: {roleClaimValue}, accountId: {accountId}");
                                switch (roleClaimValue.ToLower())
                                {
                                    case "customer":
                                        // For customers, get the customer ID based on account ID
                                        await SetCustomerSession(context, accountId);
                                        break;
                                    case "supporter":
                                        // For supporters, get the supporter ID based on account ID
                                        await SetSupporterSession(context, accountId);
                                        break;
                                    default:
                                        Console.WriteLine($"No session setting needed for role: {roleClaimValue}");
                                        break;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("AccountId not found or invalid in JWT claims");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Token validation failed, clear any existing sessions
                    context.Session.Remove("CustomerId");
                    context.Session.Remove("SupporterId");
                    Console.WriteLine($"JWT validation failed: {ex.Message}");
                }
            }
            else
            {
                // No token, clear sessions
                context.Session.Remove("CustomerId");
                context.Session.Remove("SupporterId");
            }

            await _next(context);
        }

        private async Task SetCustomerSession(HttpContext context, int accountId)
        {
            try
            {
                Console.WriteLine($"SetCustomerSession called with accountId: {accountId}");
                // Call API to get customer ID from account ID
                var httpClient = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
                var url = $"{apiBaseUrl}/Customer/by-account/{accountId}";
                Console.WriteLine($"Calling API: {url}");
                
                var response = await httpClient.GetAsync(url);
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response Content: {content}");
                    
                    var customer = System.Text.Json.JsonSerializer.Deserialize<CustomerResponse>(content, new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                    
                    if (customer != null && customer.CustomerId > 0)
                    {
                        context.Session.SetInt32("CustomerId", customer.CustomerId);
                        Console.WriteLine($"CustomerId session set to: {customer.CustomerId}");
                    }
                    else
                    {
                        Console.WriteLine("Customer object is null or CustomerId is 0");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API call failed: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set customer session: {ex.Message}");
                Console.WriteLine($"Exception details: {ex}");
            }
        }

        private async Task SetSupporterSession(HttpContext context, int accountId)
        {
            try
            {
                Console.WriteLine($"SetSupporterSession called with accountId: {accountId}");
                // Call API to get supporter ID from account ID
                var httpClient = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
                var url = $"{apiBaseUrl}/Supporter/by-account/{accountId}";
                Console.WriteLine($"Calling API: {url}");
                
                var response = await httpClient.GetAsync(url);
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response Content: {content}");
                    
                    var supporter = System.Text.Json.JsonSerializer.Deserialize<SupporterResponse>(content, new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                    
                    if (supporter != null && supporter.SupporterId > 0)
                    {
                        context.Session.SetInt32("SupporterId", supporter.SupporterId);
                        Console.WriteLine($"SupporterId session set to: {supporter.SupporterId}");
                    }
                    else
                    {
                        Console.WriteLine("Supporter object is null or SupporterId is 0");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API call failed: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set supporter session: {ex.Message}");
                Console.WriteLine($"Exception details: {ex}");
            }
        }
    }

    // Helper classes for API responses
    public class CustomerResponse
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string? CustomerCode { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }

    public class SupporterResponse
    {
        public int SupporterId { get; set; }
        public int AccountId { get; set; }
        public string? SupporterCode { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
    }
}
