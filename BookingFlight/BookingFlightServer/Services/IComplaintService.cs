using BookingFlightServer.DTO.Request;
using BookingFlightServer.DTO.Response;

namespace BookingFlightServer.Services
{
    public interface IComplaintService
    {
        Task<ComplaintResponseDto> CreateComplaintAsync(ComplaintCreateRequestDto request);
        Task<IEnumerable<ComplaintResponseDto>> GetComplaintsByCustomerAsync(int customerId);
        Task<IEnumerable<ComplaintResponseDto>> GetAllComplaintsAsync();
        Task<ComplaintResponseDto?> GetComplaintByIdAsync(int complaintId);
        Task<IEnumerable<ComplaintResponseDto>> GetComplaintsByStatusAsync(int statusId);
        Task<bool> AssignSupporterAsync(int complaintId, int supporterId);
        Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId);
        Task<string?> SaveAttachmentFileAsync(IFormFile file);
    }
}
