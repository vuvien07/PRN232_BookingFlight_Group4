using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintService(IComplaintRepository complaintRepository, IWebHostEnvironment webHostEnvironment)
        {
            _complaintRepository = complaintRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ComplaintResponseDto> CreateComplaintAsync(ComplaintCreateRequestDto request)
        {
            try
            {
                var complaint = new Complaint
                {
                    Description = request.Description,
                    CustomerId = request.CustomerId,
                    StatusId = 3, // 3 is "Pending" status
                    SupporterId = 1, // Default supporter or can be assigned later
                    CreateAt = DateTime.Now
                };

                // Handle file upload if exists
                if (request.AttachmentFile != null)
                {
                    var filePath = await SaveAttachmentFileAsync(request.AttachmentFile);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        complaint.FileUrl = filePath;
                        complaint.FileType = Path.GetExtension(request.AttachmentFile.FileName);
                        // complaint.FileName = request.AttachmentFile.FileName; // Column doesn't exist in DB
                    }
                }

                await _complaintRepository.Create(complaint);

                // Get the created complaint with related data
                var createdComplaint = await _complaintRepository.GetComplaintByIdAsync(complaint.ComplaintId);
                return MapToResponseDto(createdComplaint!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating complaint: {ex.Message} | Inner: {ex.InnerException?.Message}", ex);
            }
        }

        public async Task<IEnumerable<ComplaintResponseDto>> GetComplaintsByCustomerAsync(int customerId)
        {
            var complaints = await _complaintRepository.GetComplaintsByCustomerIdAsync(customerId);
            return complaints.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<ComplaintResponseDto>> GetAllComplaintsAsync()
        {
            var complaints = await _complaintRepository.GetAllComplaintsAsync();
            return complaints.Select(MapToResponseDto);
        }

        public async Task<ComplaintResponseDto?> GetComplaintByIdAsync(int complaintId)
        {
            var complaint = await _complaintRepository.GetComplaintByIdAsync(complaintId);
            return complaint != null ? MapToResponseDto(complaint) : null;
        }

        public async Task<IEnumerable<ComplaintResponseDto>> GetComplaintsByStatusAsync(int statusId)
        {
            var complaints = await _complaintRepository.GetComplaintsByStatusAsync(statusId);
            return complaints.Select(MapToResponseDto);
        }

        public async Task<bool> AssignSupporterAsync(int complaintId, int supporterId)
        {
            try
            {
                await _complaintRepository.AssignSupporterAsync(complaintId, supporterId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId)
        {
            try
            {
                await _complaintRepository.UpdateComplaintStatusAsync(complaintId, statusId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> SaveAttachmentFileAsync(IFormFile file)
        {
            try
            {
                // Handle case where WebRootPath might be null
                var webRootPath = _webHostEnvironment.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    // Fallback to ContentRootPath if WebRootPath is null
                    webRootPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
                }

                var uploadsFolder = Path.Combine(webRootPath, "uploads", "complaints");
                
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Shorter file name to avoid URL length issues
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return shorter URL path
                return $"/File/Complaints/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving file: {ex.Message}", ex);
            }
        }

        private ComplaintResponseDto MapToResponseDto(Complaint complaint)
        {
            return new ComplaintResponseDto
            {
                ComplaintId = complaint.ComplaintId,
                Description = complaint.Description,
                CreateAt = complaint.CreateAt,
                FileType = complaint.FileType,
                FileUrl = complaint.FileUrl,
                // FileName = complaint.FileName, // Column doesn't exist in DB
                StatusId = complaint.StatusId,
                StatusName = complaint.Status?.StatusName ?? "Unknown",
                CustomerId = complaint.CustomerId,
                CustomerName = complaint.Customer?.Fullname ?? "Unknown",
                CustomerEmail = complaint.Customer?.Email ?? "Unknown",
                SupporterId = complaint.SupporterId,
                SupporterName = complaint.Supporter?.Fullname,
                SupporterEmail = complaint.Supporter?.Email
            };
        }
    }
}
