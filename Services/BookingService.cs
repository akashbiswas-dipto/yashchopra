using System.Linq;
using BrisbaneAirportSimple.Models;

namespace BrisbaneAirportSimple.Services
{
    public static class BookingService
    {
        public static Ticket BookFlightInteractive(User user, System.Collections.Generic.List<Flight> flightlist)
        {
            Console.WriteLine("Available flights:");
            for (int i = 0; i < flightlist.Count; i++)
                Console.WriteLine($"{i + 1}. {flightlist[i].FlightCode} to {flightlist[i].City}");

            Console.Write("Choose flight number: ");
            int fl = int.Parse(Console.ReadLine()!);
            Flight flight = flightlist[fl - 1];

            Console.WriteLine("Available seats:");
            flight.ShowSeats();

            Console.Write("Choose seat code (or leave empty for auto): ");
            string? seatcode = Console.ReadLine();

            Seat seat = null!;
            if (!string.IsNullOrEmpty(seatcode))
            {
                seat = flight.Plane.Seats.FirstOrDefault(s => s.Code == seatcode && !s.IsBooked)!;
                if (seat == null && user is FrequentFlyer)
                    seat = flight.Plane.Seats.FirstOrDefault(s => !s.IsBooked)!;
            }
            if (seat == null)
                seat = flight.Plane.Seats.FirstOrDefault(s => !s.IsBooked)!;

            if (seat == null) { Console.WriteLine("No seats available!"); return null!; }

            seat.IsBooked = true;

            if (user is FrequentFlyer ff) ff.Points += 10;

            Ticket ticket = new Ticket(user, flight, seat.Code);
            Console.WriteLine($"Booked seat {seat.Code} on {flight.FlightCode}");
            return ticket;
        }
    }
}
