using BookingFlightClient.Models.ViewModels;
using BookingFlightClient.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingFlightClient.Controllers
{
    public class ComplaintController : Controller
    {
        private readonly IComplaintService _complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        // GET: /Complaint
        public async Task<IActionResult> Index()
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (!customerId.HasValue)
                {
                    return RedirectToAction("Index", "Login");
                }

                var complaints = await _complaintService.GetComplaintsByCustomerAsync(customerId.Value);
                return View(complaints);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(new List<ComplaintViewModel>());
            }
        }

        // GET: /Complaint/Create
        public async Task<IActionResult> Create()
        {
            var customerId = await GetCurrentCustomerId();
            if (!customerId.HasValue)
            {
                return RedirectToAction("Index", "Login");
            }

            var model = new ComplaintCreateViewModel
            {
                CustomerId = customerId.Value
            };

            return View(model);
        }

        // POST: /Complaint/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComplaintCreateViewModel model)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (!customerId.HasValue)
                {
                    return RedirectToAction("Index", "Login");
                }

                model.CustomerId = customerId.Value;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Validate file if exists
                if (model.AttachmentFile != null)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
                    var fileExtension = Path.GetExtension(model.AttachmentFile.FileName).ToLower();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("AttachmentFile", "Chỉ chấp nhận file ảnh (jpg, png) hoặc tài liệu (pdf, doc, docx)");
                        return View(model);
                    }

                    if (model.AttachmentFile.Length > 5 * 1024 * 1024) // 5MB
                    {
                        ModelState.AddModelError("AttachmentFile", "File không được vượt quá 5MB");
                        return View(model);
                    }
                }

                var result = await _complaintService.CreateComplaintAsync(model);
                TempData["SuccessMessage"] = "Khiếu nại của bạn đã được gửi thành công. Chúng tôi sẽ xử lý trong thời gian sớm nhất.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
        }

        // GET: /Complaint/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var customerId = await GetCurrentCustomerId();
                if (!customerId.HasValue)
                {
                    return RedirectToAction("Index", "Login");
                }

                var complaint = await _complaintService.GetComplaintByIdAsync(id);
                if (complaint == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy khiếu nại";
                    return RedirectToAction(nameof(Index));
                }

                // Check if the complaint belongs to the current customer
                if (complaint.CustomerId != customerId.Value)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem khiếu nại này";
                    return RedirectToAction(nameof(Index));
                }

                return View(complaint);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Complaint/Help - Public page for creating complaints
        public IActionResult Help()
        {
            return View();
        }

        /// <summary>
        /// Get current customer ID from session/authentication
        /// </summary>
        /// <returns>Customer ID or null</returns>
        private async Task<int?> GetCurrentCustomerId()
        {
            try
            {
                // Check if user is authenticated
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return null;
                }

                // Get token from cookie to make API call
                string? accessToken = null;
                
                if (Request.Headers.TryGetValue("Cookie", out var cookieHeaderValues))
                {
                    var rawCookieString = string.Join("; ", cookieHeaderValues);
                    var tokenMatch = System.Text.RegularExpressions.Regex.Match(rawCookieString, @"X-Access-Token=([^;]+)");
                    if (tokenMatch.Success)
                    {
                        accessToken = tokenMatch.Groups[1].Value;
                    }
                }

                if (string.IsNullOrEmpty(accessToken))
                {
                    // Fallback to try Request.Cookies
                    accessToken = Request.Cookies["X-Access-Token"];
                }

                if (string.IsNullOrEmpty(accessToken))
                {
                    return null;
                }

                // Make API call to get current customer (using the new API endpoint)
                using var httpClient = new HttpClient();
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:5077/api/Customer/GetCurrent");
                requestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
                requestMessage.Headers.Add("Cookie", $"X-Access-Token={accessToken}");
                
                var response = await httpClient.SendAsync(requestMessage);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var customer = System.Text.Json.JsonSerializer.Deserialize<CustomerResponse>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return customer?.CustomerId;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                // Log exception if needed
                Console.WriteLine($"GetCurrentCustomerId Exception: {ex.Message}");
                return null;
            }
        }

        // Helper class for API response
        private class CustomerResponse
        {
            public int CustomerId { get; set; }
            public string? Username { get; set; }
            public string? Fullname { get; set; }
            public string? Email { get; set; }
        }
    }
}
