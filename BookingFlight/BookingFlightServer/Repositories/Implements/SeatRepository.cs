using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    // Repository implementation cho quản lý ghế
    public class SeatRepository : ISeatRepository
    {
        private readonly BookingFlightContext _context;

        public SeatRepository(BookingFlightContext context)
        {
            _context = context;
        }

        // Lấy tất cả ghế với thông tin liên quan
        public async Task<IEnumerable<Seat>> GetAllSeatsAsync()
        {
            return await _context.Seats
                .Include(s => s.Class)    // Hạng ghế: Economy, Business
                .Include(s => s.Status)   // Trạng thái: Available, Booked
                .Include(s => s.Plane)    // Máy bay: Boeing 787
                .OrderBy(s => s.PlaneId)  // Sắp xếp theo máy bay
                .ThenBy(s => s.SeatNumber) // Rồi theo số ghế: 1A, 1B, 2A
                .ToListAsync();
        }

        // Lấy ghế theo máy bay - cho filter
        public async Task<IEnumerable<Seat>> GetSeatsByPlaneIdAsync(int planeId)
        {
            return await _context.Seats
                .Include(s => s.Class)
                .Include(s => s.Status) 
                .Include(s => s.Plane)
                .Where(s => s.PlaneId == planeId) // Filter theo máy bay
                .OrderBy(s => s.SeatNumber) // Sắp xếp 1A, 1B, 1C, 2A...
                .ToListAsync();
        }

        // Lấy chi tiết ghế với FlightSeats để tính thống kê
        public async Task<Seat?> GetSeatByIdAsync(int seatId)
        {
            return await _context.Seats
                .Include(s => s.Class)
                .Include(s => s.Status)
                .Include(s => s.Plane)
                .Include(s => s.FlightSeats) // Để tính số lần booking
                .FirstOrDefaultAsync(s => s.SeatId == seatId);
        }

        // Tạo ghế đơn lẻ
        public async Task<bool> CreateSeatAsync(Seat seat)
        {
            try
            {
                _context.Seats.Add(seat);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Tạo nhiều ghế cùng lúc - cho máy bay mới
        public async Task<bool> BulkCreateSeatsAsync(List<Seat> seats)
        {
            try
            {
                _context.Seats.AddRange(seats); // Thêm hàng loạt
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Cập nhật ghế - chỉ cho phép đổi ClassId và StatusId
        public async Task<bool> UpdateSeatAsync(Seat seat)
        {
            try
            {
                _context.Seats.Update(seat);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        // Xóa ghế với logic kiểm tra booking history
        public async Task<bool> DeleteSeatAsync(int seatId)
        {
            try
            {
                var seat = await _context.Seats
                    .Include(s => s.FlightSeats)
                    .FirstOrDefaultAsync(s => s.SeatId == seatId);
                    
                if (seat == null) return false;

                // Kiểm tra có lịch sử booking không
                bool hasBookingHistory = seat.FlightSeats.Any(fs => fs.TicketId != null);
                
                if (hasBookingHistory)
                {
                    // Đánh dấu "Removed" thay vì xóa thật
                    var removedStatus = await GetStatusByNameAsync("Removed", "Seat");
                    if (removedStatus != null)
                    {
                        seat.StatusId = removedStatus.StatusId;
                        _context.Seats.Update(seat);
                        return await _context.SaveChangesAsync() > 0;
                    }
                    return false;
                }
                else
                {
                    // An toàn để xóa - không có lịch sử booking
                    _context.Seats.Remove(seat);
                    return await _context.SaveChangesAsync() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // Kiểm tra số ghế đã tồn tại trong máy bay chưa
        public async Task<bool> SeatNumberExistsAsync(string seatNumber, int planeId)
        {
            return await _context.Seats
                .AnyAsync(s => s.SeatNumber == seatNumber && s.PlaneId == planeId);
        }

        // Kiểm tra ghế có lịch sử booking không
        public async Task<bool> HasBookingHistoryAsync(int seatId)
        {
            return await _context.FlightSeats
                .AnyAsync(fs => fs.SeatId == seatId && fs.TicketId != null);
        }

        // Lấy danh sách hạng ghế cho dropdown
        public async Task<IEnumerable<ClassSeat>> GetClassSeatsAsync()
        {
            return await _context.ClassSeats
                .OrderBy(c => c.ClassName) // Economy trước, Business sau
                .ToListAsync();
        }

        // Lấy trạng thái ghế cho dropdown  
        public async Task<IEnumerable<Status>> GetSeatStatusesAsync()
        {
            return await _context.Statuses
                .Where(s => s.StatusType == "Seat") // Chỉ lấy status của Seat
                .OrderBy(s => s.StatusName)
                .ToListAsync();
        }

        // Lấy danh sách máy bay cho dropdown
        public async Task<IEnumerable<Plane>> GetPlanesAsync()
        {
            return await _context.Planes
                .OrderBy(p => p.PlaneCode) // VN123, VN456...
                .ToListAsync();
        }

        // Tìm status theo tên và loại
        public async Task<Status?> GetStatusByNameAsync(string statusName, string statusType)
        {
            return await _context.Statuses
                .FirstOrDefaultAsync(s => s.StatusName == statusName && 
                                        s.StatusType == statusType);
        }
    }
}
