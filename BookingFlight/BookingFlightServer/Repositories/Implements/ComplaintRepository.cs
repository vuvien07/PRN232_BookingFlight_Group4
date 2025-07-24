using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class ComplaintRepository : BaseRepository<Complaint>, IComplaintRepository
    {
        public ComplaintRepository(BookingFlightContext repositoryDbContext) : base(repositoryDbContext)
        {
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByCustomerIdAsync(int customerId)
        {
            return await FindByCondition(
                c => c.CustomerId == customerId,
                includes: c => c.Include(x => x.Customer)
                              .Include(x => x.Status)
                              .Include(x => x.Supporter),
                trackChanges: false
            ).OrderByDescending(c => c.CreateAt).ToListAsync();
        }

        public async Task<IEnumerable<Complaint>> GetAllComplaintsAsync()
        {
            return await FindAll(
                includes: c => c.Include(x => x.Customer)
                              .Include(x => x.Status)
                              .Include(x => x.Supporter),
                trackChanges: false
            ).OrderByDescending(c => c.CreateAt).ToListAsync();
        }

        public async Task<Complaint?> GetComplaintByIdAsync(int complaintId)
        {
            return await GetByCondition(
                c => c.ComplaintId == complaintId,
                includes: c => c.Include(x => x.Customer)
                              .Include(x => x.Status)
                              .Include(x => x.Supporter),
                trackChanges: false
            );
        }

        public async Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(int statusId)
        {
            return await FindByCondition(
                c => c.StatusId == statusId,
                includes: c => c.Include(x => x.Customer)
                              .Include(x => x.Status)
                              .Include(x => x.Supporter),
                trackChanges: false
            ).OrderByDescending(c => c.CreateAt).ToListAsync();
        }

        public async Task AssignSupporterAsync(int complaintId, int supporterId)
        {
            var complaint = await GetByCondition(c => c.ComplaintId == complaintId, trackChanges: true);
            if (complaint != null)
            {
                complaint.SupporterId = supporterId;
                await Update(complaint);
            }
        }

        public async Task UpdateComplaintStatusAsync(int complaintId, int statusId)
        {
            var complaint = await GetByCondition(c => c.ComplaintId == complaintId, trackChanges: true);
            if (complaint != null)
            {
                complaint.StatusId = statusId;
                await Update(complaint);
            }
        }
    }
}
