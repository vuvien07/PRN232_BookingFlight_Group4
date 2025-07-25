using BookingFlightServer.DTO.Request;
using BookingFlightServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateComplaint([FromForm] ComplaintCreateRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _complaintService.CreateComplaintAsync(request);
                return Ok(new { success = true, data = result, message = "Khiếu nại đã được tạo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi tạo khiếu nại", error = ex.Message });
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetComplaintsByCustomer(int customerId)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByCustomerAsync(customerId);
                return Ok(new { success = true, data = complaints });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy danh sách khiếu nại", error = ex.Message });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Supporter")]
        public async Task<IActionResult> GetAllComplaints()
        {
            try
            {
                var complaints = await _complaintService.GetAllComplaintsAsync();
                return Ok(new { success = true, data = complaints });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy danh sách khiếu nại", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplaintById(int id)
        {
            try
            {
                var complaint = await _complaintService.GetComplaintByIdAsync(id);
                if (complaint == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy khiếu nại" });
                }

                return Ok(new { success = true, data = complaint });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy thông tin khiếu nại", error = ex.Message });
            }
        }

        [HttpGet("status/{statusId}")]
        [Authorize(Roles = "Admin,Supporter")]
        public async Task<IActionResult> GetComplaintsByStatus(int statusId)
        {
            try
            {
                var complaints = await _complaintService.GetComplaintsByStatusAsync(statusId);
                return Ok(new { success = true, data = complaints });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy danh sách khiếu nại", error = ex.Message });
            }
        }

        [HttpPut("{id}/assign-supporter/{supporterId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignSupporter(int id, int supporterId)
        {
            try
            {
                var result = await _complaintService.AssignSupporterAsync(id, supporterId);
                if (result)
                {
                    return Ok(new { success = true, message = "Đã gán nhân viên hỗ trợ thành công" });
                }
                return BadRequest(new { success = false, message = "Không thể gán nhân viên hỗ trợ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi gán nhân viên hỗ trợ", error = ex.Message });
            }
        }

        [HttpPut("{id}/status/{statusId}")]
        [Authorize(Roles = "Admin,Supporter")]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            try
            {
                var result = await _complaintService.UpdateComplaintStatusAsync(id, statusId);
                if (result)
                {
                    return Ok(new { success = true, message = "Đã cập nhật trạng thái thành công" });
                }
                return BadRequest(new { success = false, message = "Không thể cập nhật trạng thái" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi cập nhật trạng thái", error = ex.Message });
            }
        }
    }
}
