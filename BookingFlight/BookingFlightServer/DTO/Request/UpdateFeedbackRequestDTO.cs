using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
    public class UpdateFeedbackRequestDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Account ID is required")]
        public int AccountId { get; set; }
    }
}
