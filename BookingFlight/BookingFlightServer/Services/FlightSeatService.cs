using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services
{
    public class FlightSeatService
    {
        private readonly IFlightSeatRepository _flightSeatRepository;

        public FlightSeatService(IFlightSeatRepository flightSeatRepository)
        {
            _flightSeatRepository = flightSeatRepository;
        }

        public async Task<List<FlightSeat>> GetFlightSeatsAsync(int flightId)
        {
            // Thử lấy dữ liệu thật từ database trước
            var realSeats = await _flightSeatRepository.GetFlightSeatsByFlightId(flightId);
            
            // Nếu không có dữ liệu thật, trả về dữ liệu mẫu 150 ghế
            if (realSeats == null || !realSeats.Any())
            {
                return await GenerateMockSeatsAsync(flightId);
            }
            
            return realSeats;
        }

        public async Task<FlightSeat?> GetFlightSeatByIdAsync(int flightId, int seatId)
        {
            return await _flightSeatRepository.GetFlightSeatByIdAsync(flightId, seatId);
        }

        public async Task<bool> UpdateFlightSeatStatusAsync(int flightId, int seatId, bool isSat, int? ticketId = null)
        {
            return await _flightSeatRepository.UpdateFlightSeatStatusAsync(flightId, seatId, isSat, ticketId);
        }

        public async Task<bool> AssignSeatToTicketAsync(int flightId, int seatId, int ticketId)
        {
            return await _flightSeatRepository.AssignSeatToTicketAsync(flightId, seatId, ticketId);
        }

        public async Task<bool> UnassignSeatAsync(int flightId, int seatId)
        {
            return await _flightSeatRepository.UnassignSeatAsync(flightId, seatId);
        }

        public async Task<List<FlightSeat>> GetAvailableSeatsAsync(int flightId)
        {
            return await _flightSeatRepository.GetAvailableFlightSeatsAsync(flightId);
        }

        public async Task<List<FlightSeat>> GetOccupiedSeatsAsync(int flightId)
        {
            return await _flightSeatRepository.GetOccupiedFlightSeatsAsync(flightId);
        }

        public async Task<(int available, int occupied)> GetSeatStatisticsAsync(int flightId)
        {
            var availableCount = await _flightSeatRepository.GetAvailableSeatCountAsync(flightId);
            var occupiedCount = await _flightSeatRepository.GetOccupiedSeatCountAsync(flightId);
            return (availableCount, occupiedCount);
        }

        // Tạo 150 ghế mẫu cho chuyến bay
        public async Task<List<FlightSeat>> GenerateMockSeatsAsync(int flightId)
        {
            var seats = new List<FlightSeat>();
            var random = new Random();
            
            // Tạo 25 hàng x 6 ghế = 150 ghế
            var seatId = 1;
            for (int row = 1; row <= 25; row++)
            {
                var positions = new[] { "A", "B", "C", "D", "E", "F" };
                
                foreach (var position in positions)
                {
                    // Tạo trạng thái ghế ngẫu nhiên
                    bool isOccupied = random.NextDouble() < 0.3; // 30% ghế đã được đặt
                    bool isAvailable = !isOccupied && random.NextDouble() < 0.9; // 90% ghế trống có thể đặt
                    
                    var seat = new FlightSeat
                    {
                        FlightId = flightId,
                        SeatId = seatId++,
                        IsSat = isOccupied,
                        TicketId = isOccupied ? random.Next(1, 100) : null,
                        Seat = new Seat
                        {
                            SeatId = seatId - 1,
                            SeatNumber = $"{row}{position}",
                            ClassId = row <= 3 ? 1 : (row <= 6 ? 2 : 3), // Business, Premium, Economy
                            StatusId = isAvailable ? 1 : (isOccupied ? 2 : 3), // Available, Occupied, Unavailable
                            PlaneId = 1
                        }
                    };
                    
                    seats.Add(seat);
                }
            }
            
            return seats;
        }

        public async Task<bool> CreateSeatsForFlightAsync(int flightId)
        {
            return await _flightSeatRepository.CreateFlightSeatsForFlightAsync(flightId);
        }

        public async Task<bool> BulkAssignSeatsAsync(int flightId, List<(int seatId, int ticketId)> assignments)
        {
            try
            {
                foreach (var (seatId, ticketId) in assignments)
                {
                    var success = await _flightSeatRepository.AssignSeatToTicketAsync(flightId, seatId, ticketId);
                    if (!success) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BulkUnassignSeatsAsync(int flightId, List<int> seatIds)
        {
            try
            {
                foreach (var seatId in seatIds)
                {
                    var success = await _flightSeatRepository.UnassignSeatAsync(flightId, seatId);
                    if (!success) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
