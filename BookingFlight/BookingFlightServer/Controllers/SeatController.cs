using BookingFlightServer.DTO.Seat;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize] // Tạm thời bỏ để test
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // GET: api/Seat - Lấy tất cả ghế
        [HttpGet]
        public async Task<ActionResult<SeatListDTO>> GetAllSeats()
        {
            try
            {
                var result = await _seatService.GetAllSeatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // GET: api/Seat/plane/{planeId} - Lấy ghế theo máy bay
        [HttpGet("plane/{planeId}")]
        public async Task<ActionResult<SeatListDTO>> GetSeatsByPlane(int planeId)
        {
            try
            {
                var result = await _seatService.GetSeatsByPlaneIdAsync(planeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // GET: api/Seat/{id} - Lấy chi tiết ghế
        [HttpGet("{id}")]
        public async Task<ActionResult<SeatDetailDTO>> GetSeatDetail(int id)
        {
            try
            {
                var result = await _seatService.GetSeatByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Không tìm thấy ghế" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // POST: api/Seat - Tạo ghế mới
        [HttpPost]
        public async Task<ActionResult> CreateSeat([FromBody] CreateSeatDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var (success, message) = await _seatService.CreateSeatAsync(request);
                
                if (success)
                {
                    return Ok(new { message });
                }
                else
                {
                    return BadRequest(new { message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // POST: api/Seat/bulk - Tạo ghế hàng loạt
        [HttpPost("bulk")]
        public async Task<ActionResult> BulkCreateSeats([FromBody] BulkCreateSeatDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var (success, message) = await _seatService.BulkCreateSeatsAsync(request);
                
                if (success)
                {
                    return Ok(new { message });
                }
                else
                {
                    return BadRequest(new { message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // PUT: api/Seat/{id} - Cập nhật ghế
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSeat(int id, [FromBody] UpdateSeatDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var (success, message) = await _seatService.UpdateSeatAsync(id, request);
                
                if (success)
                {
                    return Ok(new { message });
                }
                else
                {
                    return BadRequest(new { message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // DELETE: api/Seat/{id} - Xóa ghế
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSeat(int id)
        {
            try
            {
                var (success, message) = await _seatService.DeleteSeatAsync(id);
                
                if (success)
                {
                    return Ok(new { message });
                }
                else
                {
                    return BadRequest(new { message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // GET: api/Seat/form-data - Lấy data cho form (dropdown)
        [HttpGet("form-data")]
        public async Task<ActionResult<SeatFormDataDTO>> GetFormData()
        {
            try
            {
                var result = await _seatService.GetFormDataAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi server: {ex.Message}" });
            }
        }

        // POST: api/Seat/seed-data - Tạo dữ liệu mẫu cho test
        [HttpPost("seed-data")]
        public async Task<ActionResult> SeedSampleData()
        {
            try
            {
                await _seatService.CreateSampleDataAsync();
                return Ok(new { message = "Đã tạo dữ liệu mẫu thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi tạo dữ liệu mẫu: {ex.Message}" });
            }
        }
    }
}
