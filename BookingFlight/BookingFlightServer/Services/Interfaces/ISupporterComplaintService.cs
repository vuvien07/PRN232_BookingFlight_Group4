using BookingFlightServer.DTO.Response;
using BookingFlightServer.Entities;

namespace BookingFlightServer.Services.Interfaces
{
    public interface ISupporterComplaintService
    {
        Task<IEnumerable<ComplaintResponseDto>> GetComplaintsForSupporterAsync();
        Task<ComplaintResponseDto?> GetComplaintByIdAsync(int complaintId);
        Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId);
        Task<bool> AssignComplaintToSupporterAsync(int complaintId, int supporterId);
        Task<IEnumerable<Complaint>> GetComplaintsBySupporterAsync(int supporterId);
        Task<string> GenerateAIResponseSuggestionAsync(string complaintDescription);
        Task<bool> SendResolutionEmailAsync(string customerEmail, string customerName, 
            string subject, string content);
        Task<ComplaintStatisticsDTO> GetComplaintStatisticsAsync();
    }

    public class ComplaintStatisticsDTO
    {
        public int TotalComplaints { get; set; }
        public int PendingComplaints { get; set; }
        public int ProcessingComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int AssignedToSupporterComplaints { get; set; }
    }
}
