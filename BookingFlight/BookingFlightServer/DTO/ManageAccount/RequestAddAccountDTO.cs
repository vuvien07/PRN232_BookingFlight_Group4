using System.ComponentModel.DataAnnotations;

namespace BookingFlightServer.DTO.ManageAccount
{
    public class RequestAddAccountDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public int StatusId { get; set; }
    }
}
