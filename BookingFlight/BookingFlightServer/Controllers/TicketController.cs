using BookingFlightServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

		public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Route("getByTicketNumber")]
		[HttpGet]
        public async Task<IActionResult> GetByTicketNumber([FromQuery] string? ticketNumber)
        {
            return Ok(await _ticketService.GetByTicketNumber(ticketNumber));
        }

        [Route("getAllTickets")]
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            return Ok(await _ticketService.GetAllTickets());
        }

        [Route("getTicketsPaginated")]
        [HttpGet]
        public async Task<IActionResult> GetTicketsPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _ticketService.GetTicketsPaginated(page, pageSize));
        }

        [Route("getTicketsByStatus/{statusId}")]
        [HttpGet]
        public async Task<IActionResult> GetTicketsByStatus(int statusId)
        {
            return Ok(await _ticketService.GetTicketsByStatus(statusId));
        }

        [Route("getTicketsByDateRange")]
        [HttpGet]
        public async Task<IActionResult> GetTicketsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return Ok(await _ticketService.GetTicketsByDateRange(startDate, endDate));
        }

        [Route("getTicketById/{ticketId}")]
        [HttpGet]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {
            var ticket = await _ticketService.GetTicketById(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }
            return Ok(ticket);
        }

        [Route("updateTicketStatus")]
        [HttpPut]
        public async Task<IActionResult> UpdateTicketStatus([FromQuery] int ticketId, [FromQuery] int statusId)
        {
            var result = await _ticketService.UpdateTicketStatus(ticketId, statusId);
            if (result)
            {
                return Ok(new { message = "Ticket status updated successfully" });
            }
            return BadRequest(new { message = "Failed to update ticket status" });
        }

        [Route("deleteTicket/{ticketId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var result = await _ticketService.DeleteTicket(ticketId);
            if (result)
            {
                return Ok(new { message = "Ticket deleted successfully" });
            }
            return BadRequest(new { message = "Failed to delete ticket" });
        }

        [Route("getByCustomerId/{customerId}")]
        [HttpGet]
        public async Task<IActionResult> GetTicketsByCustomerId(int customerId)
        {
            return Ok(await _ticketService.GetTicketsByCustomerId(customerId));
        }

        [Route("getByCustomerIdPaginated/{customerId}")]
        [HttpGet]
        public async Task<IActionResult> GetTicketsByCustomerIdPaginated(int customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _ticketService.GetTicketsByCustomerIdPaginated(customerId, page, pageSize));
        }
    }
}
