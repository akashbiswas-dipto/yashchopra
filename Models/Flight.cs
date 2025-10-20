using System;

namespace BrisbaneAirportSimple.Models
{
    public class Flight
    {
        public string Airline;
        public string FlightCode;
        public string City; // Departure or Arrival city
        public Plane Plane;
        public DateTime ScheduledDateTime;
        public string Status = "On Time";

        public Flight(string airline, string flightCode, string city, Plane plane, DateTime scheduledDateTime)
        {
            Airline = airline;
            FlightCode = flightCode;
            City = city;
            Plane = plane;
            ScheduledDateTime = scheduledDateTime;
        }

        public void ShowSeats()
        {
            foreach (var s in Plane.Seats)
            {
                string mark = s.IsBooked ? "[X]" : "[ ]";
                Console.Write($"{s.Code}{mark} ");
            }
            Console.WriteLine();
        }
    }
}
