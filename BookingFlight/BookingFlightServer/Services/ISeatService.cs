using BookingFlightServer.DTO.Seat;

namespace BookingFlightServer.Services
{
    // Service interface cho quản lý ghế
    public interface ISeatService
    {
        // === CRUD OPERATIONS ===
        Task<SeatListDTO> GetAllSeatsAsync(); // Lấy tất cả ghế với thống kê
        Task<SeatListDTO> GetSeatsByPlaneIdAsync(int planeId); // Lấy ghế theo máy bay
        Task<SeatDetailDTO?> GetSeatByIdAsync(int seatId); // Lấy chi tiết ghế
        Task<(bool Success, string Message)> CreateSeatAsync(CreateSeatDTO request); // Tạo ghế đơn lẻ
        Task<(bool Success, string Message)> BulkCreateSeatsAsync(BulkCreateSeatDTO request); // Tạo ghế hàng loạt
        Task<(bool Success, string Message)> UpdateSeatAsync(int seatId, UpdateSeatDTO request); // Cập nhật ghế
        Task<(bool Success, string Message)> DeleteSeatAsync(int seatId); // Xóa ghế

        // === HELPER METHODS ===
        Task<SeatFormDataDTO> GetFormDataAsync(); // Lấy data cho form (dropdown)
        Task CreateSampleDataAsync(); // Tạo dữ liệu mẫu
    }
}
