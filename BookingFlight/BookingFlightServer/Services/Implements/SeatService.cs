using BookingFlightServer.DTO.Seat;
using BookingFlightServer.Entities;
using BookingFlightServer.Repositories;

namespace BookingFlightServer.Services.Implements
{
    // Service implementation cho quản lý ghế
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;

        public SeatService(ISeatRepository seatRepository)
        {
            _seatRepository = seatRepository;
        }

        // Lấy tất cả ghế với thống kê
        public async Task<SeatListDTO> GetAllSeatsAsync()
        {
            var seats = await _seatRepository.GetAllSeatsAsync();
            
            var seatDTOs = seats.Select(s => new SeatDTO
            {
                SeatId = s.SeatId,
                SeatNumber = s.SeatNumber,
                ClassId = s.ClassId,
                ClassName = s.Class?.ClassName ?? "",
                ClassPrice = s.Class != null ? s.Class.Price : 0m,
                StatusId = s.StatusId,
                StatusName = s.Status?.StatusName ?? "",
                PlaneId = s.PlaneId,
                PlaneName = s.Plane?.PlaneCode ?? ""
            }).ToList();

            return new SeatListDTO
            {
                Seats = seatDTOs,
                TotalCount = seatDTOs.Count
            };
        }

        // Lấy ghế theo máy bay
        public async Task<SeatListDTO> GetSeatsByPlaneIdAsync(int planeId)
        {
            var seats = await _seatRepository.GetSeatsByPlaneIdAsync(planeId);
            
            var seatDTOs = seats.Select(s => new SeatDTO
            {
                SeatId = s.SeatId,
                SeatNumber = s.SeatNumber,
                ClassId = s.ClassId,
                ClassName = s.Class?.ClassName ?? "",
                ClassPrice = s.Class != null ? s.Class.Price : 0m,
                StatusId = s.StatusId,
                StatusName = s.Status?.StatusName ?? "",
                PlaneId = s.PlaneId,
                PlaneName = s.Plane?.PlaneCode ?? ""
            }).ToList();

            return new SeatListDTO
            {
                Seats = seatDTOs,
                TotalCount = seatDTOs.Count,
                FilterPlaneId = planeId
            };
        }

        // Lấy chi tiết ghế với thống kê booking
        public async Task<SeatDetailDTO?> GetSeatByIdAsync(int seatId)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null) return null;

            // Tính thống kê booking
            var totalBookings = seat.FlightSeats.Count(fs => fs.TicketId != null);
            var hasBookingHistory = totalBookings > 0;

            return new SeatDetailDTO
            {
                SeatId = seat.SeatId,
                SeatNumber = seat.SeatNumber,
                ClassName = seat.Class?.ClassName ?? "",
                ClassPrice = seat.Class != null ? seat.Class.Price : 0m,
                StatusName = seat.Status?.StatusName ?? "",
                PlaneName = seat.Plane?.PlaneCode ?? "",
                TotalBookings = totalBookings,
                HasBookingHistory = hasBookingHistory
            };
        }

        // Tạo ghế đơn lẻ với validation
        public async Task<(bool Success, string Message)> CreateSeatAsync(CreateSeatDTO request)
        {
            // Kiểm tra số ghế đã tồn tại chưa
            var exists = await _seatRepository.SeatNumberExistsAsync(request.SeatNumber, request.PlaneId);
            if (exists)
            {
                return (false, $"Số ghế {request.SeatNumber} đã tồn tại trong máy bay này");
            }

            var seat = new Seat
            {
                SeatNumber = request.SeatNumber.ToUpper(), // Chuyển thành chữ hoa: 1a -> 1A
                ClassId = request.ClassId,
                StatusId = request.StatusId,
                PlaneId = request.PlaneId
            };

            var success = await _seatRepository.CreateSeatAsync(seat);
            return success 
                ? (true, "Tạo ghế thành công")
                : (false, "Lỗi khi tạo ghế");
        }

        // Tạo ghế hàng loạt cho máy bay mới
        public async Task<(bool Success, string Message)> BulkCreateSeatsAsync(BulkCreateSeatDTO request)
        {
            var seats = new List<Seat>();
            
            // Lấy ClassId cho Business và Economy
            var classes = await _seatRepository.GetClassSeatsAsync();
            var businessClass = classes.FirstOrDefault(c => c.ClassName.ToLower().Contains("business"));
            var economyClass = classes.FirstOrDefault(c => c.ClassName.ToLower().Contains("economy"));
            
            // Lấy StatusId cho Available
            var statuses = await _seatRepository.GetSeatStatusesAsync();
            var availableStatus = statuses.FirstOrDefault(s => s.StatusName.ToLower().Contains("available"));
            
            if (businessClass == null || economyClass == null || availableStatus == null)
            {
                return (false, "Không tìm thấy hạng ghế hoặc trạng thái phù hợp");
            }

            // Tạo ghế Business Class: Hàng 1 đến BusinessRows
            for (int row = 1; row <= request.BusinessRows; row++)
            {
                foreach (char letter in request.SeatLayout)
                {
                    // Kiểm tra ghế đã tồn tại chưa
                    string seatNumber = $"{row}{letter}";
                    var exists = await _seatRepository.SeatNumberExistsAsync(seatNumber, request.PlaneId);
                    if (!exists)
                    {
                        seats.Add(new Seat
                        {
                            SeatNumber = seatNumber,
                            ClassId = businessClass.ClassId,
                            PlaneId = request.PlaneId,
                            StatusId = availableStatus.StatusId
                        });
                    }
                }
            }
            
            // Tạo ghế Economy Class: Hàng (BusinessRows + 1) đến (BusinessRows + EconomyRows)
            for (int row = request.BusinessRows + 1; row <= (request.BusinessRows + request.EconomyRows); row++)
            {
                foreach (char letter in request.SeatLayout)
                {
                    string seatNumber = $"{row}{letter}";
                    var exists = await _seatRepository.SeatNumberExistsAsync(seatNumber, request.PlaneId);
                    if (!exists)
                    {
                        seats.Add(new Seat
                        {
                            SeatNumber = seatNumber,
                            ClassId = economyClass.ClassId,
                            PlaneId = request.PlaneId,
                            StatusId = availableStatus.StatusId
                        });
                    }
                }
            }

            if (seats.Count == 0)
            {
                return (false, "Tất cả ghế đã tồn tại trong máy bay này");
            }

            var success = await _seatRepository.BulkCreateSeatsAsync(seats);
            return success 
                ? (true, $"Tạo thành công {seats.Count} ghế")
                : (false, "Lỗi khi tạo ghế hàng loạt");
        }

        // Cập nhật ghế với validation
        public async Task<(bool Success, string Message)> UpdateSeatAsync(int seatId, UpdateSeatDTO request)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null)
            {
                return (false, "Không tìm thấy ghế");
            }

            // Cập nhật thông tin (không cho đổi SeatNumber và PlaneId)
            seat.ClassId = request.ClassId;
            seat.StatusId = request.StatusId;

            var success = await _seatRepository.UpdateSeatAsync(seat);
            return success 
                ? (true, "Cập nhật ghế thành công")
                : (false, "Lỗi khi cập nhật ghế");
        }

        // Xóa ghế với logic kiểm tra booking history
        public async Task<(bool Success, string Message)> DeleteSeatAsync(int seatId)
        {
            var seat = await _seatRepository.GetSeatByIdAsync(seatId);
            if (seat == null)
            {
                return (false, "Không tìm thấy ghế");
            }

            // Kiểm tra ghế có lịch sử booking không
            var hasBookingHistory = seat.FlightSeats.Any(fs => fs.TicketId != null);
            
            var success = await _seatRepository.DeleteSeatAsync(seatId);
            
            if (success)
            {
                if (hasBookingHistory)
                {
                    return (true, "Ghế đã được đánh dấu là 'Removed' do có lịch sử booking");
                }
                else
                {
                    return (true, "Xóa ghế thành công");
                }
            }
            
            return (false, "Lỗi khi xóa ghế");
        }

        // Lấy data cho form - danh sách máy bay, hạng ghế, trạng thái
        public async Task<SeatFormDataDTO> GetFormDataAsync()
        {
            var planes = await _seatRepository.GetPlanesAsync();
            var classes = await _seatRepository.GetClassSeatsAsync();
            var statuses = await _seatRepository.GetSeatStatusesAsync();

            return new SeatFormDataDTO
            {
                Planes = planes.Select(p => new PlaneOptionDTO
                {
                    PlaneId = p.PlaneId ?? 0,
                    PlaneName = p.PlaneCode ?? "",
                    PlaneModel = p.Model ?? "",
                    CurrentSeats = 0 // TODO: Tính từ database
                }).ToList(),
                
                Classes = classes.Select(c => new ClassOptionDTO
                {
                    ClassId = c.ClassId,
                    ClassName = c.ClassName ?? "",
                    Price = c.Price
                }).ToList(),
                
                Statuses = statuses.Select(s => new StatusOptionDTO
                {
                    StatusId = s.StatusId,
                    StatusName = s.StatusName ?? ""
                }).ToList()
            };
        }

        // Tạo dữ liệu mẫu cho test
        public async Task CreateSampleDataAsync()
        {
            // Kiểm tra xem đã có dữ liệu chưa
            var existingSeats = await _seatRepository.GetAllSeatsAsync();
            if (existingSeats.Any())
            {
                return; // Đã có dữ liệu rồi
            }

            // Tạo dữ liệu mẫu
            var sampleSeats = new List<Seat>();
            
            // Giả sử có PlaneId = 1, ClassId = 1 (Economy), StatusId = 1 (Available)
            for (int row = 1; row <= 30; row++)
            {
                foreach (char letter in "ABCDEF")
                {
                    sampleSeats.Add(new Seat
                    {
                        SeatNumber = $"{row}{letter}",
                        PlaneId = 1,
                        ClassId = 1,
                        StatusId = row % 4 == 0 ? 2 : 1 // Mỗi 4 hàng có 1 ghế booked
                    });
                }
            }

            // Business class (hàng 31-35)
            for (int row = 31; row <= 35; row++)
            {
                foreach (char letter in "ABCD")
                {
                    sampleSeats.Add(new Seat
                    {
                        SeatNumber = $"{row}{letter}",
                        PlaneId = 1,
                        ClassId = 2, // Business
                        StatusId = 1 // Available
                    });
                }
            }

            // Thêm vào database
            foreach (var seat in sampleSeats)
            {
                await _seatRepository.CreateSeatAsync(seat);
            }
        }
    }
}
