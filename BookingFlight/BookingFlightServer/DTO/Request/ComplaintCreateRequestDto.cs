using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.Request
{
    public class ComplaintCreateRequestDto
    {
        [Required(ErrorMessage = "Mô tả khiếu nại không được để trống")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; } = null!;

        public IFormFile? AttachmentFile { get; set; }

        public int CustomerId { get; set; }
    }
}
