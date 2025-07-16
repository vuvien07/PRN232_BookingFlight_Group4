namespace BookingFlightServer.DTO.ManageAccount
{
    public class ResponseAccountDTO
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public int StatusId { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }

}
