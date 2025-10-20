using System;
using System.Collections.Generic;
using System.Linq;

namespace BrisbaneAirportSimple.Models
{
    public enum FlightDirection { Arrival, Departure }

    public class Flight
    {
        public string Airline;
        public string FlightId;
        public string City;
        public Plane Plane;
        public DateTime ScheduledDateTime;
        public FlightDirection Direction;
        public string Status = "On Time";
        public Dictionary<string, Ticket> SeatMap = new Dictionary<string, Ticket>();

        public Flight(string airline, string flightId, string city, Plane plane, DateTime scheduled, FlightDirection dir)
        {
            Airline = airline;
            FlightId = flightId;
            City = city;
            Plane = plane;
            ScheduledDateTime = scheduled;
            Direction = dir;
        }

        public IEnumerable<Seat> AvailableSeats() => Plane.Seats.Where(s => !s.IsBooked);

        public void ShowSeats()
        {
            foreach (var s in Plane.Seats)
            {
                var mark = s.IsBooked ? "[X]" : "[ ]";
                System.Console.Write($"{s.Code}{mark} ");
            }
            System.Console.WriteLine();
        }
    }
}
