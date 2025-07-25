using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories.Interfaces
{
    public interface ISupporterComplaintRepository
    {
        Task<IEnumerable<Complaint>> GetComplaintsForSupporterAsync();
        Task<Complaint?> GetComplaintByIdAsync(int complaintId);
        Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId);
        Task<bool> AssignComplaintToSupporterAsync(int complaintId, int supporterId);
        Task<IEnumerable<Complaint>> GetComplaintsBySupporterAsync(int supporterId);
    }
}
