using System.ComponentModel.DataAnnotations;

namespace BookingFlightClient.Models.ViewModels
{
    public class ComplaintCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mô tả khiếu nại")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        [Display(Name = "Mô tả khiếu nại")]
        public string Description { get; set; } = null!;

        [Display(Name = "File đính kèm")]
        public IFormFile? AttachmentFile { get; set; }

        public int CustomerId { get; set; }
    }
}
