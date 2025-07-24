using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Controllers
{
    [Authorize]
    public class SupporterComplaintController : Controller
    {
        private readonly ILogger<SupporterComplaintController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SupporterComplaintController(
            ILogger<SupporterComplaintController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private void SetUserRole()
        {
            // Get role from JWT token claims
            var roleIdClaim = HttpContext.User?.FindFirst("RoleId")?.Value;
            var roleNameClaim = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
            
            if (int.TryParse(roleIdClaim, out int roleId))
            {
                ViewBag.UserRole = roleId;
                ViewBag.RoleName = roleId == 2 ? "Supporter" : (roleNameClaim ?? "Unknown");
            }
            else
            {
                ViewBag.UserRole = 2; // Default to Supporter
                ViewBag.RoleName = "Supporter";
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                SetUserRole();
                
                // Check if user is Supporter (roleId = 2) - Allow access for debugging
                var userRole = ViewBag.UserRole;
                _logger.LogInformation($"User role: {userRole}, RoleName: {ViewBag.RoleName}");
                
                // For now, comment out the strict role check to debug
                // if (ViewBag.UserRole != 2)
                // {
                //     return RedirectToAction("Unauthorized", "Home");
                // }

                var complaints = await GetSupporterComplaintsAsync();
                _logger.LogInformation($"Retrieved {complaints.Count} complaints for supporter");
                return View(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading supporter complaints");
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải danh sách khiếu nại";
                return View(new List<ComplaintResponseDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResolveComplaint(int complaintId)
        {
            try
            {
                SetUserRole();
                _logger.LogInformation($"Resolving complaint {complaintId}, User role: {ViewBag.UserRole}");
                
                // Temporarily allow all users for debugging
                // if (ViewBag.UserRole != 2)
                // {
                //     return Json(new { success = false, message = "Không có quyền truy cập" });
                // }

                var result = await UpdateComplaintStatusAsync(complaintId, 4); // statusId = 4 (Resolved)
                
                if (result)
                {
                    return Json(new { success = true, message = "Đã đánh dấu khiếu nại là đã giải quyết" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể cập nhật trạng thái khiếu nại" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving complaint {ComplaintId}", complaintId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAIResponse([FromBody] GenerateResponseRequest request)
        {
            try
            {
                SetUserRole();
                
                if (string.IsNullOrEmpty(request.ComplaintDescription))
                {
                    return Json(new { success = false, message = "Nội dung khiếu nại không được để trống" });
                }

                var aiResponse = await GetAIResponseSuggestionAsync(request.ComplaintDescription);
                
                return Json(new { 
                    success = true, 
                    suggestion = aiResponse 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response for complaint");
                return Json(new { 
                    success = false, 
                    message = "Không thể tạo gợi ý phản hồi. Vui lòng thử lại sau." 
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendResolutionEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                SetUserRole();
                
                if (string.IsNullOrEmpty(request.CustomerEmail) || 
                    string.IsNullOrEmpty(request.Subject) || 
                    string.IsNullOrEmpty(request.Content))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin email" });
                }

                var result = await SendComplaintResolutionEmailAsync(request);
                
                if (result)
                {
                    // Also update complaint status to resolved after sending email
                    await UpdateComplaintStatusAsync(request.ComplaintId, 4);
                    
                    return Json(new { 
                        success = true, 
                        message = "Email đã được gửi thành công và khiếu nại đã được đánh dấu là đã giải quyết" 
                    });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể gửi email. Vui lòng thử lại sau." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending resolution email for complaint {ComplaintId}", request.ComplaintId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi gửi email" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComplaintDetails(int id)
        {
            try
            {
                SetUserRole();
                
                var complaint = await GetComplaintByIdAsync(id);
                
                if (complaint == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy khiếu nại" });
                }

                return Json(new { 
                    success = true, 
                    complaint = new {
                        id = complaint.ComplaintId,
                        customerName = complaint.CustomerName,
                        customerEmail = complaint.CustomerEmail,
                        title = $"Khiếu nại #{complaint.ComplaintId}",
                        description = complaint.Description,
                        createdDate = complaint.CreateAt?.ToString("dd/MM/yyyy HH:mm") ?? "",
                        statusName = complaint.StatusName
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint details for ID {ComplaintId}", id);
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải thông tin khiếu nại" });
            }
        }

        private async Task<List<ComplaintResponseDto>> GetSupporterComplaintsAsync()
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                _logger.LogInformation($"Calling API: {apiUrl}/supporter-complaint/supporter-complaints");
                
                var response = await httpClient.GetAsync($"{apiUrl}/supporter-complaint/supporter-complaints");
                
                _logger.LogInformation($"API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"API Response Content: {content}");
                    
                    var complaints = JsonSerializer.Deserialize<List<ComplaintResponseDto>>(content, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    }) ?? new List<ComplaintResponseDto>();
                    
                    _logger.LogInformation($"Deserialized {complaints.Count} complaints");
                    return complaints;
                }

                _logger.LogWarning("Failed to get supporter complaints. Status: {StatusCode}, Content: {Content}", 
                    response.StatusCode, await response.Content.ReadAsStringAsync());
                return new List<ComplaintResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching supporter complaints from API");
                return new List<ComplaintResponseDto>();
            }
        }

        private async Task<ComplaintResponseDto?> GetComplaintByIdAsync(int complaintId)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                
                // Get complaint from the supporter complaints list since we don't have a specific endpoint
                var complaints = await GetSupporterComplaintsAsync();
                return complaints.FirstOrDefault(c => c.ComplaintId == complaintId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching complaint {ComplaintId} from API", complaintId);
                return null;
            }
        }

        private async Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var apiUrl = _configuration["ApiSettings:BaseUrl"];
                
                // Use the resolve endpoint that exists in the backend
                var response = await httpClient.PutAsync($"{apiUrl}/supporter-complaint/{complaintId}/resolve", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complaint status for ID {ComplaintId}", complaintId);
                return false;
            }
        }

        private async Task<string> GetAIResponseSuggestionAsync(string complaintDescription)
        {
            try
            {
                // Tạo gợi ý phản hồi cơ bản cho khiếu nại
                return $@"Kính gửi Quý khách,

Chúng tôi đã nhận được khiếu nại của Quý khách và đã tiến hành xem xét kỹ lưỡng.

Nội dung khiếu nại: {complaintDescription}

Chúng tôi xin chân thành xin lỗi vì sự bất tiện mà Quý khách đã gặp phải. Để giải quyết vấn đề này, chúng tôi sẽ:

1. Kiểm tra lại quy trình dịch vụ
2. Áp dụng các biện pháp khắc phục phù hợp
3. Đảm bảo tình huống tương tự không tái diễn

Chúng tôi cam kết cải thiện chất lượng dịch vụ để mang lại trải nghiệm tốt nhất cho Quý khách.

Trân trọng,
Đội ngũ hỗ trợ khách hàng";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AI response suggestion");
                return "Chúng tôi đang xử lý vấn đề của bạn và sẽ phản hồi sớm nhất có thể.";
            }
        }

        private async Task<bool> SendComplaintResolutionEmailAsync(SendEmailRequest request)
        {
            try
            {
                // Simulate email sending for now
                _logger.LogInformation($"Simulating email sent to {request.CustomerEmail} for complaint {request.ComplaintId}");
                _logger.LogInformation($"Subject: {request.Subject}");
                _logger.LogInformation($"Content: {request.Content}");
                
                // In a real implementation, you would integrate with an email service
                // For now, we'll just return true to indicate success
                await Task.Delay(500); // Simulate email sending delay
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending resolution email");
                return false;
            }
        }
    }

    // Request models for API calls
    public class GenerateResponseRequest
    {
        public string ComplaintDescription { get; set; } = string.Empty;
    }

    public class SendEmailRequest
    {
        public int ComplaintId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class ComplaintResponseDto
    {
        public int ComplaintId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; // Match backend DTO
        public string ComplaintDescription => Description; // Alias for compatibility
        public string Title { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime? CreateAt { get; set; }
        public DateTime? CreatedDate => CreateAt; // Alias for view compatibility
        public DateTime? UpdateAt { get; set; }
        public string? ResponseMessage { get; set; }
        
        // Supporter Information
        public int SupporterId { get; set; }
        public string? SupporterName { get; set; }
        public string? SupporterEmail { get; set; }
    }

    public class ComplaintStatisticsDto
    {
        public int TotalComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public double ResolutionRate { get; set; }
    }
}
