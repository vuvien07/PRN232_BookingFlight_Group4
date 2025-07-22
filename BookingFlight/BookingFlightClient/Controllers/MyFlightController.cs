using Microsoft.AspNetCore.Mvc;
using BookingFlightClient.Models.DTO.Customer;

namespace BookingFlightClient.Controllers
{
    [Route("MyFlights")]
    public class MyFlightController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public MyFlightController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiBaseUrl = configuration.GetSection("ApiSettings")["BaseUrl"] ?? "http://localhost:5077/api";
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] int? year, [FromQuery] int? month)
        {
            ViewBag.Year = year ?? DateTime.Now.Year;
            ViewBag.Month = month ?? DateTime.Now.Month;
            return View("~/Views/MyFlights/Index.cshtml");
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> Calendar([FromQuery] int? year, [FromQuery] int? month)
        {
            ViewBag.Year = year ?? DateTime.Now.Year;
            ViewBag.Month = month ?? DateTime.Now.Month;
            return View("~/Views/MyFlights/Calendar.cshtml");
        }

        [HttpGet("detail/{ticketId}")]
        public async Task<IActionResult> Detail(int ticketId)
        {
            ViewBag.TicketId = ticketId;
            return View("~/Views/MyFlights/Detail.cshtml");
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> Upcoming()
        {
            return View("~/Views/MyFlights/Upcoming.cshtml");
        }

        [HttpGet("past")]
        public async Task<IActionResult> Past()
        {
            return View("~/Views/MyFlights/Past.cshtml");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? searchTerm)
        {
            ViewBag.SearchTerm = searchTerm ?? "";
            return View("~/Views/MyFlights/Search.cshtml");
        }

        // Enhanced debug endpoint to troubleshoot cookie parsing
        [HttpGet("api/debug-cookies")]
        public IActionResult DebugCookies()
        {
            try
            {
                // Gather all debug information
                var allCookies = Request.Cookies.ToDictionary(c => c.Key, c => c.Value);
                var cookieKeys = Request.Cookies.Keys.ToList();
                
                // Check raw Cookie header
                var rawCookieHeader = Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues) 
                    ? string.Join("; ", cookieHeaderValues) 
                    : "No Cookie header";
                
                // Check for X-Access-Token in different ways
                var hasXAccessToken1 = Request.Cookies.ContainsKey("X-Access-Token");
                var hasXAccessToken2 = Request.Cookies.Any(c => c.Key == "X-Access-Token");
                var hasXAccessToken3 = Request.Cookies.Any(c => c.Key.Equals("X-Access-Token", StringComparison.OrdinalIgnoreCase));
                var tokenFromCookies = Request.Cookies.TryGetValue("X-Access-Token", out var tokenValue) ? tokenValue : null;
                
                // Try to extract from raw header
                string? tokenFromRawHeader = null;
                var hasInRawHeader = rawCookieHeader.Contains("X-Access-Token=");
                if (hasInRawHeader)
                {
                    var match = System.Text.RegularExpressions.Regex.Match(rawCookieHeader, @"X-Access-Token=([^;]+)");
                    if (match.Success)
                    {
                        tokenFromRawHeader = match.Groups[1].Value;
                    }
                }
                
                return Ok(new
                {
                    success = true,
                    cookieParsingDebug = new
                    {
                        cookieCount = Request.Cookies.Count,
                        allCookieKeys = cookieKeys,
                        hasXAccessTokenContainsKey = hasXAccessToken1,
                        hasXAccessTokenAny = hasXAccessToken2,
                        hasXAccessTokenCaseInsensitive = hasXAccessToken3,
                        tokenFromCookiesCollection = tokenFromCookies?.Substring(0, Math.Min(50, tokenFromCookies.Length)) + "...",
                        rawCookieHeader = rawCookieHeader.Substring(0, Math.Min(200, rawCookieHeader.Length)) + "...",
                        hasXAccessTokenInRawHeader = hasInRawHeader,
                        tokenFromRawHeader = tokenFromRawHeader?.Substring(0, Math.Min(50, tokenFromRawHeader.Length)) + "...",
                        issue = !hasXAccessToken1 && hasInRawHeader ? "Cookie sent by browser but not parsed by ASP.NET Core" : "Normal operation"
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
            }
        }

        // Proxy login endpoint to get token on client domain
        [HttpPost("api/proxy-login")]
        public async Task<IActionResult> ProxyLogin([FromBody] LoginRequest request)
        {
            try
            {
                Console.WriteLine($"[MyFlights Proxy Login] Attempting login for user: {request.Username}");
                
                // Forward login request to server
                var loginData = System.Text.Json.JsonSerializer.Serialize(new 
                {
                    username = request.Username,
                    password = request.Password
                });
                
                var loginRequest = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5077/api/Login/login")
                {
                    Content = new StringContent(loginData, System.Text.Encoding.UTF8, "application/json")
                };
                
                Console.WriteLine($"[MyFlights Proxy Login] Forwarding to: {loginRequest.RequestUri}");
                
                var response = await _httpClient.SendAsync(loginRequest);
                var content = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"[MyFlights Proxy Login] Server response: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[MyFlights Proxy Login] ✅ Login successful: {content}");
                    
                    // Check for Set-Cookie headers from server
                    if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
                    {
                        Console.WriteLine($"[MyFlights Proxy Login] Found Set-Cookie headers: {string.Join("; ", setCookieHeaders)}");
                        
                        foreach (var setCookieHeader in setCookieHeaders)
                        {
                            if (setCookieHeader.Contains("X-Access-Token="))
                            {
                                // Extract and set the token for client domain
                                var tokenMatch = System.Text.RegularExpressions.Regex.Match(setCookieHeader, @"X-Access-Token=([^;]+)");
                                if (tokenMatch.Success)
                                {
                                    var tokenValue = tokenMatch.Groups[1].Value;
                                    
                                    // Set the cookie for client domain
                                    Response.Cookies.Append("X-Access-Token", tokenValue, new CookieOptions
                                    {
                                        HttpOnly = true,
                                        Secure = false,
                                        SameSite = SameSiteMode.Lax,
                                        Path = "/"
                                    });
                                    
                                    Console.WriteLine($"[MyFlights Proxy Login] ✅ Set X-Access-Token cookie for client domain");
                                }
                            }
                        }
                    }
                    
                    return Ok(new { success = true, message = "Login successful", redirectUrl = "/MyFlights" });
                }
                else
                {
                    Console.WriteLine($"[MyFlights Proxy Login] ❌ Login failed: {content}");
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Login failed", details = content });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Proxy Login] Exception: {ex.Message}");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        public class LoginRequest
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // Test server authentication endpoint
        [HttpGet("api/test-auth")]
        public async Task<IActionResult> TestServerAuth()
        {
            try
            {
                Console.WriteLine("[MyFlights Test Auth] Testing server authentication...");
                
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}/MyFlight/auth-test");
                
                // Forward all cookies
                if (Request.Cookies.Any())
                {
                    var cookieHeader = string.Join("; ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
                    requestMessage.Headers.Add("Cookie", cookieHeader);
                    Console.WriteLine($"[MyFlights Test Auth] Forwarding cookies: {cookieHeader}");
                }

                var response = await _httpClient.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[MyFlights Test Auth] Server response status: {response.StatusCode}");
                Console.WriteLine($"[MyFlights Test Auth] Server response: {content}");

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Test Auth] Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Alternative auth check - redirect to server if needed
        [HttpGet("api/auth-status")]
        public async Task<IActionResult> CheckAuthStatus()
        {
            try
            {
                Console.WriteLine("[MyFlights Auth Check] Checking authentication status...");
                
                // Check if we have X-Access-Token cookie on client
                if (Request.Cookies.TryGetValue("X-Access-Token", out var clientToken))
                {
                    Console.WriteLine($"[MyFlights Auth Check] ✅ Found X-Access-Token on client: {clientToken.Substring(0, Math.Min(30, clientToken.Length))}...");
                    return Ok(new { authenticated = true, source = "client-cookie", hasToken = true });
                }
                
                // Try to call server API to check if user is authenticated there
                Console.WriteLine("[MyFlights Auth Check] No client token, checking server...");
                
                var tokenRequest = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5077/api/Token/get");
                
                // Forward any existing cookies
                if (Request.Cookies.Any())
                {
                    var cookieHeader = string.Join("; ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
                    tokenRequest.Headers.Add("Cookie", cookieHeader);
                    Console.WriteLine($"[MyFlights Auth Check] Forwarding cookies to server: {cookieHeader}");
                }

                var response = await _httpClient.SendAsync(tokenRequest);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MyFlights Auth Check] ✅ Server confirms authentication: {content.Substring(0, Math.Min(100, content.Length))}...");
                    
                    // User is authenticated on server but not on client
                    return Ok(new { 
                        authenticated = true, 
                        source = "server-only", 
                        hasToken = false,
                        message = "User is authenticated on server but token not available on client domain",
                        suggestion = "User needs to login through client domain"
                    });
                }
                else
                {
                    Console.WriteLine($"[MyFlights Auth Check] ❌ Server authentication check failed: {response.StatusCode}");
                    return Ok(new { 
                        authenticated = false, 
                        source = "none",
                        hasToken = false,
                        message = "User not authenticated on either client or server"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Auth Check] Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("api/sync-auth")]
        public async Task<IActionResult> SyncAuth()
        {
            try
            {
                Console.WriteLine("[MyFlights Auth Sync] Starting auth synchronization...");
                
                // Check current cookies on client
                Console.WriteLine($"[MyFlights Auth Sync] Current client cookies: {string.Join("; ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"))}");
                
                // Build the request to server's Token/get endpoint
                var tokenRequest = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5077/api/Token/get");
                
                // Forward any existing cookies from the current request
                if (Request.Cookies.Any())
                {
                    var cookieHeader = string.Join("; ", Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
                    tokenRequest.Headers.Add("Cookie", cookieHeader);
                    Console.WriteLine($"[MyFlights Auth Sync] Forwarding cookies to server: {cookieHeader}");
                }

                Console.WriteLine($"[MyFlights Auth Sync] Calling: {tokenRequest.RequestUri}");
                var response = await _httpClient.SendAsync(tokenRequest);
                
                Console.WriteLine($"[MyFlights Auth Sync] Server response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MyFlights Auth Sync] Server token response: {content}");
                    
                    // Check if we got any Set-Cookie headers from server
                    if (response.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
                    {
                        Console.WriteLine($"[MyFlights Auth Sync] Found Set-Cookie headers: {string.Join("; ", setCookieHeaders)}");
                        
                        foreach (var setCookieHeader in setCookieHeaders)
                        {
                            if (setCookieHeader.Contains("X-Access-Token="))
                            {
                                // Parse the X-Access-Token value
                                var tokenMatch = System.Text.RegularExpressions.Regex.Match(setCookieHeader, @"X-Access-Token=([^;]+)");
                                if (tokenMatch.Success)
                                {
                                    var tokenValue = tokenMatch.Groups[1].Value;
                                    Console.WriteLine($"[MyFlights Auth Sync] Extracted token: {tokenValue.Substring(0, Math.Min(50, tokenValue.Length))}...");
                                    
                                    // Set the cookie for client domain (localhost:5001)
                                    Response.Cookies.Append("X-Access-Token", tokenValue, new CookieOptions
                                    {
                                        HttpOnly = true,
                                        Secure = false, // Set to true in production with HTTPS
                                        SameSite = SameSiteMode.Lax,
                                        Path = "/",
                                        // Don't set Domain so it defaults to current domain (localhost:5001)
                                    });
                                    
                                    Console.WriteLine($"[MyFlights Auth Sync] ✅ Successfully set X-Access-Token cookie for client domain");
                                    return Ok(new { success = true, message = "Authentication token synced successfully", tokenPreview = tokenValue.Substring(0, Math.Min(20, tokenValue.Length)) + "..." });
                                }
                            }
                        }
                    }
                    
                    // Alternative approach: Try to make a direct authenticated request to see if token works
                    Console.WriteLine("[MyFlights Auth Sync] No Set-Cookie headers found, trying alternative approach...");
                    
                    // Parse the response to see if we have token info
                    try
                    {
                        var tokenData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content);
                        
                        if (tokenData.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var nameElement))
                        {
                            var username = nameElement.GetString();
                            Console.WriteLine($"[MyFlights Auth Sync] User is authenticated as: {username}");
                            
                            // Since server confirmed authentication, let's set a dummy cookie to test
                            Response.Cookies.Append("AUTH_CONFIRMED", "true", new CookieOptions
                            {
                                HttpOnly = false,
                                Secure = false,
                                SameSite = SameSiteMode.Lax,
                                Path = "/"
                            });
                            
                            return Ok(new { success = true, message = $"User {username} is authenticated but no token cookie found", authenticated = true, username });
                        }
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"[MyFlights Auth Sync] Failed to parse token response: {parseEx.Message}");
                    }
                    
                    return Ok(new { success = false, message = "Authentication confirmed but no token cookie available" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[MyFlights Auth Sync] Server token endpoint failed: {response.StatusCode} - {errorContent}");
                    return StatusCode((int)response.StatusCode, new { error = "Failed to get token from server", details = errorContent });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Auth Sync] Exception: {ex.Message}");
                Console.WriteLine($"[MyFlights Auth Sync] Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
            }
        }

        // API Proxy methods to forward requests to server
        [HttpGet("api/stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                Console.WriteLine($"[MyFlights Stats Proxy] === COOKIE PARSING DEBUGGING ===");
                
                // Get the raw Cookie header first (this is what the browser actually sends)
                string? accessToken = null;
                
                if (Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues))
                {
                    var rawCookieString = string.Join("; ", cookieHeaderValues);
                    Console.WriteLine($"[MyFlights Stats Proxy] Raw Cookie header: {rawCookieString.Substring(0, Math.Min(200, rawCookieString.Length))}...");
                    
                    // Extract X-Access-Token from raw header (this should work)
                    var tokenMatch = System.Text.RegularExpressions.Regex.Match(rawCookieString, @"X-Access-Token=([^;]+)");
                    if (tokenMatch.Success)
                    {
                        accessToken = tokenMatch.Groups[1].Value;
                        Console.WriteLine($"[MyFlights Stats Proxy] ✅ Found X-Access-Token in raw header: {accessToken.Substring(0, Math.Min(50, accessToken.Length))}...");
                    }
                    else
                    {
                        Console.WriteLine($"[MyFlights Stats Proxy] ❌ X-Access-Token not found in raw Cookie header");
                    }
                }
                else
                {
                    Console.WriteLine($"[MyFlights Stats Proxy] ❌ No Cookie header found at all");
                }
                
                // Also try Request.Cookies for comparison
                var hasXAccessTokenInRequestCookies = Request.Cookies.ContainsKey("X-Access-Token");
                Console.WriteLine($"[MyFlights Stats Proxy] Request.Cookies.ContainsKey('X-Access-Token'): {hasXAccessTokenInRequestCookies}");
                Console.WriteLine($"[MyFlights Stats Proxy] Request.Cookies count: {Request.Cookies.Count}");
                Console.WriteLine($"[MyFlights Stats Proxy] Request.Cookies keys: {string.Join(", ", Request.Cookies.Keys)}");
                
                if (accessToken == null)
                {
                    return Unauthorized(new { 
                        error = "X-Access-Token not found",
                        details = "Could not extract X-Access-Token from Cookie header",
                        requestCookiesWorking = hasXAccessTokenInRequestCookies,
                        rawCookieHeader = Request.Headers.TryGetValue("Cookie", out var cookieVals) ? string.Join("; ", cookieVals) : "No Cookie header"
                    });
                }
                
                // Create request with proper authentication
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}/MyFlight/stats");
                
                // Forward the token using the method that works with the server
                requestMessage.Headers.Add("Cookie", $"X-Access-Token={accessToken}");
                requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
                
                Console.WriteLine($"[MyFlights Stats Proxy] Making request to: {requestMessage.RequestUri}");
                Console.WriteLine($"[MyFlights Stats Proxy] Forwarding X-Access-Token: {accessToken.Substring(0, Math.Min(30, accessToken.Length))}...");
                
                var response = await _httpClient.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[MyFlights Stats Proxy] Server response status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[MyFlights Stats Proxy] Server error response: {content}");
                }

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[MyFlights Stats Proxy] ✅ Success! Returning data to client");
                    return Content(content, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Stats Proxy] Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
            }
        }

        [HttpGet("api/flights")]
        public async Task<IActionResult> GetFlights()
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}/MyFlight/my-flights");
                
                if (Request.Headers.Cookie.Any())
                {
                    var cookieHeader = string.Join("; ", Request.Headers.Cookie);
                    requestMessage.Headers.Add("Cookie", cookieHeader);
                }

                if (Request.Headers.Authorization.Any())
                {
                    requestMessage.Headers.Add("Authorization", Request.Headers.Authorization.ToString());
                }

                var response = await _httpClient.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, content);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ==================== NEW PROXY ENDPOINTS FOR FLIGHT LISTS ====================

        [HttpGet("api/upcoming")]
        public async Task<IActionResult> GetUpcomingFlights()
        {
            return await ProxySimpleGet($"{_apiBaseUrl}/MyFlight/upcoming");
        }

        [HttpGet("api/completed")]
        public async Task<IActionResult> GetCompletedFlights([FromQuery] int limit = 100)
        {
            return await ProxySimpleGet($"{_apiBaseUrl}/MyFlight/past?limit={limit}");
        }

        [HttpGet("api/all")]
        public async Task<IActionResult> GetAllFlights()
        {
            return await ProxySimpleGet($"{_apiBaseUrl}/MyFlight/all");
        }

        [HttpGet("api/detail/{ticketId}")]
        public async Task<IActionResult> GetFlightDetail(int ticketId)
        {
            return await ProxySimpleGet($"{_apiBaseUrl}/MyFlight/detail/{ticketId}");
        }

        // Reusable helper to forward GET request with cookies and token
        private async Task<IActionResult> ProxySimpleGet(string targetUrl)
        {
            try
            {
                Console.WriteLine($"[MyFlights Proxy] Forwarding GET to {targetUrl}");

                var requestMessage = new HttpRequestMessage(HttpMethod.Get, targetUrl);

                // Forward raw Cookie header if present
                if (Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues))
                {
                    var rawCookie = string.Join("; ", cookieHeaderValues);
                    requestMessage.Headers.Add("Cookie", rawCookie);
                }

                // Forward Authorization header if present
                if (Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    requestMessage.Headers.Add("Authorization", authHeader.ToString());
                }

                var response = await _httpClient.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Proxy] Exception forwarding request: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/calendar")]
        public async Task<IActionResult> GetCalendarData([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                Console.WriteLine($"[MyFlights Calendar Proxy] Year: {year}, Month: {month}");
                
                // Use the same approach as GetStats - get token from raw Cookie header
                string? accessToken = null;
                
                if (Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues))
                {
                    var rawCookieString = string.Join("; ", cookieHeaderValues);
                    var tokenMatch = System.Text.RegularExpressions.Regex.Match(rawCookieString, @"X-Access-Token=([^;]+)");
                    if (tokenMatch.Success)
                    {
                        accessToken = tokenMatch.Groups[1].Value;
                        Console.WriteLine($"[MyFlights Calendar Proxy] ✅ Found X-Access-Token: {accessToken.Substring(0, Math.Min(50, accessToken.Length))}...");
                    }
                    else
                    {
                        Console.WriteLine($"[MyFlights Calendar Proxy] ❌ X-Access-Token not found in Cookie header");
                    }
                }
                
                if (accessToken == null)
                {
                    return Unauthorized(new { error = "X-Access-Token not found", details = "Authentication cookie missing" });
                }
                
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_apiBaseUrl}/MyFlight/calendar?year={year}&month={month}");
                requestMessage.Headers.Add("Cookie", $"X-Access-Token={accessToken}");
                requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");

                Console.WriteLine($"[MyFlights Calendar Proxy] Making request to: {requestMessage.RequestUri}");
                
                var response = await _httpClient.SendAsync(requestMessage);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"[MyFlights Calendar Proxy] Server response status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[MyFlights Calendar Proxy] Server error response: {content}");
                }

                if (response.IsSuccessStatusCode)
                {
                    return Content(content, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MyFlights Calendar Proxy] Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
            }
        }
    }
}
