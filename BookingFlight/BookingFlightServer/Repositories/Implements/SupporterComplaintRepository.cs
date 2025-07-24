using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class SupporterComplaintRepository : ISupporterComplaintRepository
    {
        private readonly BookingFlightContext _context;

        public SupporterComplaintRepository(BookingFlightContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsForSupporterAsync()
        {
            return await _context.Complaints
                .Include(c => c.Customer)
                .Include(c => c.Status)
                .Include(c => c.Supporter)
                .Where(c => c.StatusId >= 3) // Status >= 3 (Pending, Resolved, Rejected, etc.)
                .OrderByDescending(c => c.CreateAt) // Show newest first
                .ToListAsync();
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int complaintId)
        {
            return await _context.Complaints
                .Include(c => c.Customer)
                .Include(c => c.Status)
                .Include(c => c.Supporter)
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
        }

        public async Task<bool> UpdateComplaintStatusAsync(int complaintId, int statusId)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint == null)
                return false;

            // Only allow updating to Resolved status (statusId = 4)
            if (statusId != 4)
                return false;

            complaint.StatusId = statusId;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> AssignComplaintToSupporterAsync(int complaintId, int supporterId)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint == null)
                return false;

            complaint.SupporterId = supporterId;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsBySupporterAsync(int supporterId)
        {
            return await _context.Complaints
                .Include(c => c.Customer)
                .Include(c => c.Status)
                .Include(c => c.Supporter)
                .Where(c => c.SupporterId == supporterId)
                .OrderBy(c => c.CreateAt)
                .ToListAsync();
        }
    }
}
