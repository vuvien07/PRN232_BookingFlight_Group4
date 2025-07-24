using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    public interface IComplaintRepository : IBaseRepository<Complaint>
    {
        Task<IEnumerable<Complaint>> GetComplaintsByCustomerIdAsync(int customerId);
        Task<IEnumerable<Complaint>> GetAllComplaintsAsync();
        Task<Complaint?> GetComplaintByIdAsync(int complaintId);
        Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(int statusId);
        Task AssignSupporterAsync(int complaintId, int supporterId);
        Task UpdateComplaintStatusAsync(int complaintId, int statusId);
    }
}
