namespace BookingFlightServer.DTO
{
	public class FilterFeedbackDTO
	{
		public int Page { get; set; }
		public int PageSize { get; set; }
		public int Rate { get; set; }
		public long TotalFeedback { get; set; }
		public long TotalPage { get; set; }
	}
}
