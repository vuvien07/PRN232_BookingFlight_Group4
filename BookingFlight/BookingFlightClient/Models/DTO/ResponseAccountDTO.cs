namespace BookingFlightClient.Models.DTO
{
    public class ResponseAccountDTO
    {
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        //public string Password { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
    }

}
