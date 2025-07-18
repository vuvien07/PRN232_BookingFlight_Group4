using BookingFlightClient.Models.DTO;

namespace BookingFlightClient.Models.ViewModels
{
    public class ManageAccountAddVM
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public List<RoleDTO> roles { get; set; } = new List<RoleDTO>();
    }
}
