using BookingFlightServer.Entities;

namespace BookingFlightServer.Repositories
{
    // Interface repository quản lý ghế
    public interface ISeatRepository
    {
        // === CRUD OPERATIONS ===
        Task<IEnumerable<Seat>> GetAllSeatsAsync(); // Lấy tất cả ghế
        Task<IEnumerable<Seat>> GetSeatsByPlaneIdAsync(int planeId); // Lấy ghế theo máy bay
        Task<Seat?> GetSeatByIdAsync(int seatId); // Lấy chi tiết ghế
        Task<bool> CreateSeatAsync(Seat seat); // Tạo ghế đơn lẻ
        Task<bool> BulkCreateSeatsAsync(List<Seat> seats); // Tạo nhiều ghế cùng lúc
        Task<bool> UpdateSeatAsync(Seat seat); // Cập nhật ghế
        Task<bool> DeleteSeatAsync(int seatId); // Xóa ghế

        // === VALIDATION ===
        Task<bool> SeatNumberExistsAsync(string seatNumber, int planeId); // Kiểm tra số ghế tồn tại
        Task<bool> HasBookingHistoryAsync(int seatId); // Kiểm tra có lịch sử booking

        // === HELPER METHODS ===
        Task<IEnumerable<ClassSeat>> GetClassSeatsAsync(); // Lấy danh sách hạng ghế
        Task<IEnumerable<Status>> GetSeatStatusesAsync(); // Lấy trạng thái ghế
        Task<IEnumerable<Plane>> GetPlanesAsync(); // Lấy danh sách máy bay
        Task<Status?> GetStatusByNameAsync(string statusName, string statusType); // Tìm status theo tên
    }
}
