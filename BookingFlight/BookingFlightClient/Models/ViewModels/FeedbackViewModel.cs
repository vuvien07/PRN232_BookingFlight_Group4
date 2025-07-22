using System.ComponentModel.DataAnnotations;

namespace BookingFlightClient.Models.ViewModels
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Đánh giá")]
        public int Rate { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [StringLength(1000, ErrorMessage = "Nội dung không được vượt quá 1000 ký tự")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = null!;

        public int? TicketId { get; set; }
        public string? TicketNumber { get; set; }
    }

    public class FeedbackDisplayViewModel
    {
        public int FeedbackId { get; set; }
        public string Title { get; set; } = null!;
        public int Rate { get; set; }
        public string Content { get; set; } = null!;
        public DateOnly? CreateAt { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string? TicketNumber { get; set; }
    }
}
