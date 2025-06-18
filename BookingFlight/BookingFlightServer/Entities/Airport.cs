using System;
using System.Collections.Generic;

namespace BookingFlightServer.Entities;

public partial class Airport
{
    public int AirportId { get; set; }

    public string AirportCode { get; set; } = null!;

    public string AirportName { get; set; } = null!;

    public string City { get; set; } = null!;

    public int ManagerId { get; set; }

    public virtual ICollection<AirportPrice> AirportPriceAirportFroms { get; set; } = new List<AirportPrice>();

    public virtual ICollection<AirportPrice> AirportPriceAirportTos { get; set; } = new List<AirportPrice>();

    public virtual ICollection<Flight> FlightArrivalAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartureAirports { get; set; } = new List<Flight>();

    public virtual Manager Manager { get; set; } = null!;
}
