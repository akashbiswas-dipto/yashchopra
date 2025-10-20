using System;

namespace BrisbaneAirportSimple.Models
{
    public class Ticket
    {
        public Flight Flight;
        public User Passenger;
        public string SeatCode;
        public DateTime Issued;

        public Ticket(User passenger, Flight flight, string seat)
        {
            Passenger = passenger;
            Flight = flight;
            SeatCode = seat;
            Issued = DateTime.Now;
        }

        public string ShortInfo()
        {
            return $"{Passenger.Name} - {Flight.FlightId} - {Flight.City} - Seat {SeatCode} - {Flight.ScheduledDateTime} - Status:{Flight.Status}";
        }
    }
}
