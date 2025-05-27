using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entiies;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string TicketNumber { get; set; } = null!;

    public int FlightId { get; set; }

    public int StatusId { get; set; }

    public DateOnly BookingDate { get; set; }

    public int? CustomerId { get; set; }

    public decimal TotalPrice { get; set; }

    public string Gender { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string FullName { get; set; } = null!;

    public int ClassSeatId { get; set; }

    public string? ContactFullName { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactAddress { get; set; }

    public virtual ClassSeat ClassSeat { get; set; } = null!;

    public virtual Customer? Customer { get; set; }

    public virtual Flight Flight { get; set; } = null!;

    public virtual ICollection<FlightSeat> FlightSeats { get; set; } = new List<FlightSeat>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Status Status { get; set; } = null!;
}
