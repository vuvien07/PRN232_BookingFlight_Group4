using BookingFlightServer.DTO.ManageNews;
using BookingFlightServer.Services;
using BookingFlightServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingFlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = Constants.RoleAdmin)]
    public class ManageNewsController : ControllerBase
    {
        private readonly IManageNewsService manageNewsService;

        public ManageNewsController(IManageNewsService manageNewsService)
        {
            this.manageNewsService = manageNewsService;
        }

        // GET: api/managenews/news
        [HttpGet("news")]
        public async Task<IActionResult> GetAllNews()
        {
            // Call the service to get the list of news
            var responseNewsDTO = await manageNewsService.GetNewsAsync();

            // Check if the response is null or empty
            if (responseNewsDTO == null || !responseNewsDTO.Any())
            {
                // Return a NotFound response if no news is found
                return NotFound(new { message = "No news found." });
            }

            // Return the list of news
            return Ok(responseNewsDTO);
        }

        // POST: api/managenews/news
        [HttpPost("news")]
        public async Task<IActionResult> AddNews([FromBody] RequestAddNewsDTO requestAddNewsDTO)
        {
            var responseNewsDTO = await manageNewsService.CreateNewsAsync(requestAddNewsDTO);

            // Check if the news creation was successful
            if (responseNewsDTO == null)
            {
                return BadRequest(new { message = "Failed to create news." });
            }

            // Return the created news with a 201 Created status
            return StatusCode(StatusCodes.Status201Created, responseNewsDTO);
        }

        // DELETE: api/managenews/news/{newsId}
        [HttpDelete("news/{newsId}")]
        public async Task<IActionResult> DeleteNews(int newsId)
        {
            // Call the service to delete the news
            var isDeleted = await manageNewsService.DeleteNewsAsync(newsId);
            // Check if the deletion was successful
            if (!isDeleted)
            {
                return NotFound(new { message = "News not found or could not be deleted." });
            }
            // Return a NoContent response if deletion was successful
            return NoContent();
        }

        // PUT: api/managenews/news/{newsId}
        [HttpPut("news/{newsId}")]
        public async Task<IActionResult> UpdateNews(int newsId, [FromBody] RequestUpdateNewsDTO requestUpdateNewsDTO)
        {
            // Ensure the ID matches
            if (newsId != requestUpdateNewsDTO.NewId)
            {
                return BadRequest(new { message = "ID mismatch." });
            }

            // Call the service to update the news
            var isUpdated = await manageNewsService.UpdateNewsAsync(requestUpdateNewsDTO);
            // Check if the update was successful
            if (!isUpdated)
            {
                return BadRequest(new { message = "Failed to update news." });
            }
            // Return a NoContent response if update was successful
            return NoContent();
        }

        [HttpGet]
        [Route("news/{newId:int}")]
        public async Task<IActionResult> GetNewsById(int newId)
        {
            // call the service to get news by ID
            var responseNewsDTO = await manageNewsService.GetNewsByIdAsync(newId);

            // Check if the news was found
            if (responseNewsDTO == null)
            {
                return NotFound(new { message = "News not found." });
            }

            // Return the news details
            return Ok(responseNewsDTO);
        }
    }
}
