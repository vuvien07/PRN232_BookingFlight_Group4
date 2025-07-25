using System.ComponentModel.DataAnnotations;

namespace BookingFlightClient.Models.ViewModels
{
    public class FeedbackDetailViewModel
    {
        public int FeedbackId { get; set; }
        
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;
        
        [Display(Name = "Đánh giá")]
        public int Rate { get; set; }
        
        [Display(Name = "Nội dung")]
        public string Content { get; set; } = null!;
        
        [Display(Name = "Ngày tạo")]
        public DateTime? CreateAt { get; set; }
        
        // Thông tin Account
        public int AccountId { get; set; }
        
        [Display(Name = "Tên tài khoản")]
        public string AccountName { get; set; } = null!;
        
        // Thông tin Ticket
        public int TicketId { get; set; }
        
        [Display(Name = "Mã vé")]
        public string TicketNumber { get; set; } = null!;
        
        [Display(Name = "Ngày đặt")]
        public DateTime BookingDate { get; set; }
        
        [Display(Name = "Tổng tiền")]
        public decimal TotalPrice { get; set; }
        
        [Display(Name = "Tên hành khách")]
        public string PassengerName { get; set; } = null!;
        
        // Thông tin Flight
        public int FlightId { get; set; }
        
        [Display(Name = "Số hiệu chuyến bay")]
        public string FlightNumber { get; set; } = null!;
        
        [Display(Name = "Giờ khởi hành")]
        public DateTime DepartureTime { get; set; }
        
        [Display(Name = "Giờ đến")]
        public DateTime ArrivalTime { get; set; }
        
        [Display(Name = "Sân bay đi")]
        public string DepartureAirport { get; set; } = null!;
        
        [Display(Name = "Sân bay đến")]
        public string ArrivalAirport { get; set; } = null!;
        
        [Display(Name = "Ngày bay")]
        public DateTime FlightDate { get; set; }
    }
}
