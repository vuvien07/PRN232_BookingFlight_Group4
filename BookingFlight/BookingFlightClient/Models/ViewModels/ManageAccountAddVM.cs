using BookingFlightClient.Models.DTO;

namespace BookingFlightClient.Models.ViewModels
{
    public class ManageAccountAddVM
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; } = 1;

        public List<RoleDTO> roles = new List<RoleDTO>();
    }
}
