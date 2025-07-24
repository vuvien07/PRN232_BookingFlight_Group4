using BookingFlightClient.Models.ViewModels;
using System.Text;
using System.Text.Json;

namespace BookingFlightClient.Services
{
    public interface IComplaintService
    {
        Task<ComplaintViewModel> CreateComplaintAsync(ComplaintCreateViewModel model);
        Task<List<ComplaintViewModel>> GetComplaintsByCustomerAsync(int customerId);
        Task<ComplaintViewModel?> GetComplaintByIdAsync(int complaintId);
    }

    public class ComplaintService : IComplaintService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public ComplaintService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5077/api";
        }

        public async Task<ComplaintViewModel> CreateComplaintAsync(ComplaintCreateViewModel model)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.Description), "Description");
                formData.Add(new StringContent(model.CustomerId.ToString()), "CustomerId");

                if (model.AttachmentFile != null)
                {
                    var fileContent = new StreamContent(model.AttachmentFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.AttachmentFile.ContentType);
                    formData.Add(fileContent, "AttachmentFile", model.AttachmentFile.FileName);
                }

                var response = await _httpClient.PostAsync($"{_baseUrl}/Complaint/create", formData);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<ComplaintViewModel>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result?.Data ?? throw new Exception("Không thể tạo khiếu nại");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra khi tạo khiếu nại: {ex.Message}");
            }
        }

        public async Task<List<ComplaintViewModel>> GetComplaintsByCustomerAsync(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Complaint/customer/{customerId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<List<ComplaintViewModel>>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result?.Data ?? new List<ComplaintViewModel>();
                }
                else
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra khi lấy danh sách khiếu nại: {ex.Message}");
            }
        }

        public async Task<ComplaintViewModel?> GetComplaintByIdAsync(int complaintId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/Complaint/{complaintId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<ComplaintViewModel>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result?.Data;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw new Exception($"API Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Có lỗi xảy ra khi lấy thông tin khiếu nại: {ex.Message}");
            }
        }

        private class ApiResponse<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }
        }
    }
}
