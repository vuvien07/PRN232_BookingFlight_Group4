using System.ComponentModel.DataAnnotations;

namespace BookingFlightClient.Models.DTO
{
    public class RequestAddAccountDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
    }
}
