using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories.Interfaces;
using BookingFlightServer.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookingFlightServer.Services.Implements
{
    public class SupporterComplaintService : ISupporterComplaintService
    {
        private readonly ISupporterComplaintRepository _supporterComplaintRepository;
        private readonly IGeminiAIService _geminiAIService;
        private readonly IEmailService _emailService;
        private readonly ILogger<SupporterComplaintService> _logger;

        public SupporterComplaintService(
            ISupporterComplaintRepository supporterComplaintRepository,
            IGeminiAIService geminiAIService,
            IEmailService emailService,
            ILogger<SupporterComplaintService> logger)
        {
            _supporterComplaintRepository = supporterComplaintRepository;
            _geminiAIService = geminiAIService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IEnumerable<ComplaintResponseDto>> GetComplaintsForSupporterAsync()
        {
            try
            {
                _logger.LogInformation("Getting complaints for supporter");
                var complaints = await _supporterComplaintRepository.GetComplaintsForSupporterAsync();
                _logger.LogInformation($"Found {complaints.Count()} complaints from repository");
                
                var mappedComplaints = complaints.Select(MapToResponseDto).ToList();
                _logger.LogInformation($"Mapped {mappedComplaints.Count} complaints");
                
                return mappedComplaints;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints for supporter");
                return new List<ComplaintResponseDto>();
            }
        }

        public async Task<ComplaintResponseDto?> GetComplaintByIdAsync(int complaintId)
        {
            var complaint = await _supporterComplaintRepository.GetComplaintByIdAsync(complaintId);
            return complaint != null ? MapToResponseDto(complaint) : null;
        }

        public async Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId)
        {
            // Only allow updating to resolved status (statusId = 4)
            if (statusId != 4)
            {
                _logger.LogWarning("Attempt to update complaint {ComplaintId} to invalid status {StatusId}", complaintId, statusId);
                return false;
            }
            
            return await _supporterComplaintRepository.UpdateComplaintStatusAsync(complaintId, statusId);
        }

        public async Task<bool> AssignComplaintToSupporterAsync(int complaintId, int supporterId)
        {
            return await _supporterComplaintRepository.AssignComplaintToSupporterAsync(complaintId, supporterId);
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsBySupporterAsync(int supporterId)
        {
            return await _supporterComplaintRepository.GetComplaintsBySupporterAsync(supporterId);
        }

        public async Task<string> GenerateAIResponseSuggestionAsync(string complaintDescription)
        {
            try
            {
                var prompt = $@"
Bạn là một chuyên gia dịch vụ khách hàng hàng không chuyên nghiệp. Hãy tạo ra một câu trả lời hỗ trợ cho khiếu nại sau của khách hàng.

Khiếu nại: {complaintDescription}

Hãy tạo ra một phản hồi:
1. Lịch sự và chuyên nghiệp
2. Thể hiện sự hiểu biết và đồng cảm
3. Đưa ra giải pháp cụ thể nếu có thể
4. Thể hiện cam kết cải thiện dịch vụ
5. Viết bằng tiếng Việt
6. Dài khoảng 100-150 từ

Chỉ trả về nội dung phản hồi, không cần tiêu đề hay phần giải thích.";

                return await _geminiAIService.GenerateContentAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating AI response suggestion for complaint");
                return "Chúng tôi đã ghi nhận khiếu nại của quý khách và sẽ xem xét kỹ lưỡng để đưa ra giải pháp phù hợp nhất. Cảm ơn quý khách đã thông báo, chúng tôi cam kết cải thiện chất lượng dịch vụ để mang lại trải nghiệm tốt nhất cho quý khách trong những lần sử dụng dịch vụ tiếp theo.";
            }
        }

        public async Task<bool> SendResolutionEmailAsync(string customerEmail, string customerName, 
            string subject, string content)
        {
            try
            {
                await _emailService.SendEmailAsync(customerEmail, customerName, subject, content);
                _logger.LogInformation("Resolution email sent successfully to {CustomerEmail}", customerEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending resolution email to {CustomerEmail}", customerEmail);
                return false;
            }
        }

        public async Task<ComplaintStatisticsDTO> GetComplaintStatisticsAsync()
        {
            try
            {
                var allComplaints = await _supporterComplaintRepository.GetComplaintsForSupporterAsync();
                
                return new ComplaintStatisticsDTO
                {
                    TotalComplaints = allComplaints.Count(),
                    PendingComplaints = allComplaints.Count(c => c.StatusId == 1), // Pending
                    ProcessingComplaints = allComplaints.Count(c => c.StatusId == 3), // Processing/Assigned to Supporter
                    ResolvedComplaints = allComplaints.Count(c => c.StatusId == 4), // Resolved
                    AssignedToSupporterComplaints = allComplaints.Count(c => c.StatusId == 3) // Same as processing for supporter context
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint statistics");
                return new ComplaintStatisticsDTO();
            }
        }
        private ComplaintResponseDto MapToResponseDto(Complaint complaint)
        {
            return new ComplaintResponseDto
            {
                ComplaintId = complaint.ComplaintId,
                CustomerId = complaint.CustomerId,
                CustomerName = complaint.Customer?.Fullname ?? "Unknown",
                CustomerEmail = complaint.Customer?.Email ?? "",
                Description = complaint.Description,
                CreateAt = complaint.CreateAt,
                FileType = complaint.FileType,
                FileUrl = complaint.FileUrl,
                StatusId = complaint.StatusId,
                StatusName = complaint.Status?.StatusName ?? "Unknown",
                SupporterId = complaint.SupporterId,
                SupporterName = complaint.Supporter?.Fullname ?? "",
                SupporterEmail = complaint.Supporter?.Email ?? ""
            };
        }
    }
}
