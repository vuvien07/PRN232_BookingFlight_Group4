using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using BookingFlightClient.Models.DTO;

namespace BookingFlightClient.Controllers
{
    public class SeatManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public SeatManagementController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _baseApiUrl = configuration.GetValue<string>("ServerSettings:BaseUrl") ?? "http://localhost:5077";
        }

        // GET: /SeatManagement/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy danh sách ghế từ API
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Seat");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    
                    // Server trả về SeatListDTO với cấu trúc: { "seats": [...], "totalCount": 10 }
                    var serverResponse = JsonSerializer.Deserialize<ServerSeatListDTO>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return View(serverResponse?.Seats ?? new List<SeatListItemDTO>());
                }
                
                ViewBag.ErrorMessage = "Không thể tải danh sách ghế.";
                return View(new List<SeatListItemDTO>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi kết nối: {ex.Message}";
                return View(new List<SeatListItemDTO>());
            }
        }

        // GET: /SeatManagement/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Seat/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var seat = JsonSerializer.Deserialize<SeatDetailsDTO>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return View(seat);
                }
                
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: /SeatManagement/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                // Lấy dữ liệu cho form
                var formDataResponse = await _httpClient.GetAsync($"{_baseApiUrl}/api/Seat/form-data");
                
                if (formDataResponse.IsSuccessStatusCode)
                {
                    var jsonString = await formDataResponse.Content.ReadAsStringAsync();
                    var formData = JsonSerializer.Deserialize<SeatFormDataResponseDTO>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    ViewBag.FormData = formData;
                }
                
                return View(new CreateSeatRequestDTO());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi tải form: {ex.Message}";
                return View(new CreateSeatRequestDTO());
            }
        }

        // POST: /SeatManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSeatRequestDTO createSeatDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormDataAsync();
                return View(createSeatDto);
            }

            try
            {
                var json = JsonSerializer.Serialize(createSeatDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/api/Seat", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo ghế thành công!";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Lỗi tạo ghế: {errorContent}";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi kết nối: {ex.Message}";
            }

            await LoadFormDataAsync();
            return View(createSeatDto);
        }

        // GET: /SeatManagement/BulkCreate
        public async Task<IActionResult> BulkCreate()
        {
            try
            {
                await LoadFormDataAsync();
                return View(new BulkCreateSeatRequestDTO());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi tải form: {ex.Message}";
                return View(new BulkCreateSeatRequestDTO());
            }
        }

        // POST: /SeatManagement/BulkCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkCreate(BulkCreateSeatRequestDTO bulkCreateDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadFormDataAsync();
                return View(bulkCreateDto);
            }

            try
            {
                var json = JsonSerializer.Serialize(bulkCreateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/api/Seat/bulk-create", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Tạo thành công {bulkCreateDto.TotalSeats} ghế cho máy bay!";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Lỗi tạo ghế: {errorContent}";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi kết nối: {ex.Message}";
            }

            await LoadFormDataAsync();
            return View(bulkCreateDto);
        }

        // GET: /SeatManagement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Lấy thông tin ghế hiện tại
                var seatResponse = await _httpClient.GetAsync($"{_baseApiUrl}/api/Seat/{id}");
                
                if (!seatResponse.IsSuccessStatusCode)
                {
                    return NotFound();
                }
                
                var seatJson = await seatResponse.Content.ReadAsStringAsync();
                var seatDetail = JsonSerializer.Deserialize<SeatDetailsDTO>(seatJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Chuyển đổi sang UpdateSeatRequestDTO
                var updateDto = new UpdateSeatRequestDTO
                {
                    SeatId = seatDetail.SeatId,
                    SeatNumber = seatDetail.SeatNumber,
                    PlaneId = seatDetail.PlaneId,
                    ClassSeatId = seatDetail.ClassSeatId,
                    StatusId = seatDetail.StatusId
                };

                await LoadFormDataAsync();
                return View(updateDto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: /SeatManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateSeatRequestDTO updateSeatDto)
        {
            if (id != updateSeatDto.SeatId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await LoadFormDataAsync();
                return View(updateSeatDto);
            }

            try
            {
                var json = JsonSerializer.Serialize(updateSeatDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseApiUrl}/api/Seat/{id}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật ghế thành công!";
                    return RedirectToAction(nameof(Index));
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Lỗi cập nhật ghế: {errorContent}";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Lỗi kết nối: {ex.Message}";
            }

            await LoadFormDataAsync();
            return View(updateSeatDto);
        }

        // GET: /SeatManagement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/Seat/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var seat = JsonSerializer.Deserialize<SeatDetailsDTO>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return View(seat);
                }
                
                return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: /SeatManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/api/Seat/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa ghế thành công!";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Lỗi xóa ghế: {errorContent}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi kết nối: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper method để tải dữ liệu form
        private async Task LoadFormDataAsync()
        {
            try
            {
                var formDataResponse = await _httpClient.GetAsync($"{_baseApiUrl}/api/Seat/form-data");
                
                if (formDataResponse.IsSuccessStatusCode)
                {
                    var jsonString = await formDataResponse.Content.ReadAsStringAsync();
                    var formData = JsonSerializer.Deserialize<SeatFormDataResponseDTO>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    ViewBag.FormData = formData;
                }
            }
            catch (Exception)
            {
                ViewBag.FormData = new SeatFormDataResponseDTO();
            }
        }
    }

    // Client DTOs cho Seat Management
    public class ServerSeatListDTO
    {
        public List<SeatListItemDTO> Seats { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class SeatListItemDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public decimal ClassPrice { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int? PlaneId { get; set; }
        public string PlaneName { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }

    public class SeatDetailsDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int PlaneId { get; set; }
        public string PlaneName { get; set; } = string.Empty;
        public int ClassSeatId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSeatRequestDTO
    {
        public string SeatNumber { get; set; } = string.Empty;
        public int PlaneId { get; set; }
        public int ClassSeatId { get; set; }
        public int StatusId { get; set; }
    }

    public class UpdateSeatRequestDTO
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public int PlaneId { get; set; }
        public int ClassSeatId { get; set; }
        public int StatusId { get; set; }
    }

    public class BulkCreateSeatRequestDTO
    {
        public int PlaneId { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
        public int TotalSeats => EconomySeats + BusinessSeats + FirstClassSeats;
    }

    public class SeatFormDataResponseDTO
    {
        public List<PlaneOptionResponseDTO> Planes { get; set; } = new();
        public List<ClassOptionResponseDTO> Classes { get; set; } = new();
        public List<StatusOptionResponseDTO> Statuses { get; set; } = new();
    }

    public class PlaneOptionResponseDTO
    {
        public int PlaneId { get; set; }
        public string PlaneName { get; set; } = string.Empty;
    }

    public class ClassOptionResponseDTO
    {
        public int ClassSeatId { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }

    public class StatusOptionResponseDTO
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}