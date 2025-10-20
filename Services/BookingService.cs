using System;
using System.Collections.Generic;
using System.Linq;
using BrisbaneAirportSimple.Models;

namespace BrisbaneAirportSimple.Services
{
    public static class BookingService
    {
        public static Ticket? BookFlightInteractive(User user, List<Flight> flights, List<Ticket> tickets)
        {
            if (!flights.Any()) { Console.WriteLine("No flights available."); return null; }

            var list = flights.OrderBy(f => f.ScheduledDateTime).ToList();
            Console.WriteLine("Available flights:");
            for (int i = 0; i < list.Count; i++)
            {
                var f = list[i];
                Console.WriteLine($"{i + 1}. {f.FlightId} {f.Direction} {f.City} {f.ScheduledDateTime} Plane:{f.Plane.PlaneId} Status:{f.Status}");
            }

            Console.Write("Select flight number: ");
            int sel = ValidationService.ReadValidInt("", 1, list.Count);
            var flight = list[sel - 1];

            // check if user already has same direction flight
            if (user is Traveller)
            {
                bool has = tickets.Any(t => t.Passenger.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) && t.Flight.Direction == flight.Direction);
                if (has) { Console.WriteLine("You already have a flight of this direction booked."); return null; }
            }

            Console.WriteLine("Seat layout:");
            flight.ShowSeats();
            Console.Write("Enter seat (e.g. 6B) or leave empty for auto: ");
            var seatReq = (Console.ReadLine() ?? "").Trim().ToUpper();

            Seat? seat = null;

            // FrequentFlyer override logic (bump occupant if needed)
            if (user is FrequentFlyer ff && !string.IsNullOrEmpty(seatReq))
            {
                var found = flight.Plane.FindSeat(seatReq);
                if (found == null) { Console.WriteLine("Seat not found."); return null; }
                if (found.IsBooked)
                {
                    // find occupant and reassign them
                    var occ = tickets.FirstOrDefault(t => t.Flight == flight && t.SeatCode == found.Code);
                    if (occ != null)
                    {
                        var alt = FindNextAvailableSeat(flight, excludeSeatCode: found.Code);
                        if (alt == null) { Console.WriteLine("No seats to bump occupant to."); return null; }
                        // reassign occupant
                        Console.WriteLine($"Bumping {occ.Passenger.Name} from {found.Code} to {alt.Code}");
                        found.IsBooked = false;
                        alt.IsBooked = true;
                        occ.SeatCode = alt.Code;
                    }
                }
                // assign to FF
                seat = found;
                seat.IsBooked = true;
                Console.WriteLine($"Frequent Flyer {user.Name} assigned seat {seat.Code}");
            }

            // Normal allocation or fallback
            if (seat == null)
            {
                if (!string.IsNullOrEmpty(seatReq))
                {
                    var s = flight.Plane.FindSeat(seatReq);
                    if (s == null) { Console.WriteLine("Seat not found."); return null; }
                    if (!s.IsBooked)
                    {
                        seat = s;
                        seat.IsBooked = true;
                    }
                    else
                    {
                        var alt = FindNextAvailableSeat(flight, excludeSeatCode: seatReq);
                        if (alt == null) { Console.WriteLine("No seats available."); return null; }
                        seat = alt;
                        seat.IsBooked = true;
                        Console.WriteLine($"Requested seat taken; assigned {seat.Code}");
                    }
                }
                else
                {
                    var s = flight.Plane.Seats.FirstOrDefault(x => !x.IsBooked);
                    if (s == null) { Console.WriteLine("No seats available."); return null; }
                    seat = s;
                    seat.IsBooked = true;
                }
            }

            var ticket = new Ticket(user, flight, seat.Code);
            Console.WriteLine($"Ticket created: {ticket.ShortInfo()}");

            // award points for FF based on city
            if (user is FrequentFlyer fuser)
            {
                int pts = PointsForCity(flight.City);
                fuser.Points += pts;
                Console.WriteLine($"Frequent Flyer earned {pts} points. Total: {fuser.Points}");
            }

            return ticket;
        }

        private static Seat? FindNextAvailableSeat(Flight flight, string excludeSeatCode)
        {
            var lst = flight.Plane.Seats;
            int idx = lst.FindIndex(s => s.Code == excludeSeatCode);
            if (idx >= 0)
            {
                for (int i = idx + 1; i < lst.Count; i++) if (!lst[i].IsBooked) return lst[i];
                for (int i = idx - 1; i >= 0; i--) if (!lst[i].IsBooked) return lst[i];
            }
            return lst.FirstOrDefault(s => !s.IsBooked);
        }

        private static int PointsForCity(string city)
        {
            return city.ToLower() switch
            {
                "sydney" => 1200,
                "melbourne" => 1750,
                "rockhampton" => 1400,
                "adelaide" => 1950,
                "perth" => 3375,
                _ => 0
            };
        }

        public static void ShowUserTickets(User user, List<Ticket> tickets)
        {
            var my = tickets.Where(t => t.Passenger.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!my.Any()) { Console.WriteLine("No tickets found."); return; }
            foreach (var t in my) Console.WriteLine(t.ShortInfo());
        }
    }
}
