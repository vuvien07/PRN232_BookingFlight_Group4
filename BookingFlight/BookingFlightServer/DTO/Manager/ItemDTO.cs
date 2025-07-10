namespace BookingFlightServer.DTO.Manager
{
    public class ItemDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Detail { get; set; }
        public int Price { get; set; }
        public int? StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? Image { get; set; }
    }

    public class ItemListRequestDTO
    {
        public string? SearchTerm { get; set; }
        public int? StatusId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    public class ItemCreateRequestDTO
    {
        public string ItemName { get; set; } = null!;
        public string? Detail { get; set; }
        public int Price { get; set; }
        public int? StatusId { get; set; }
        public string? Image { get; set; }
    }

    public class ItemUpdateRequestDTO
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string? Detail { get; set; }
        public int Price { get; set; }
        public int? StatusId { get; set; }
        public string? Image { get; set; }
    }
}
