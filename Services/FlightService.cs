using System;
using System.Collections.Generic;
using System.Linq;
using BrisbaneAirportSimple.Models;

namespace BrisbaneAirportSimple.Services
{
    public static class FlightService
    {
        public static void AddFlightInteractive(List<Flight> flights)
        {
            Console.Write("Airline (JST/QFA/RXA/VOZ/FRE): ");
            var airline = (Console.ReadLine() ?? "").Trim().ToUpper();
            if (string.IsNullOrEmpty(airline)) { Console.WriteLine("Invalid airline."); return; }

            Console.Write("Flight ID (3 letters + 3 digits): ");
            var fid = (Console.ReadLine() ?? "").Trim().ToUpper();
            while (!ValidationService.IsValidFlightId(fid))
            {
                Console.WriteLine("Invalid Flight ID format.");
                Console.Write("Flight ID: ");
                fid = (Console.ReadLine() ?? "").Trim().ToUpper();
            }

            Console.Write("Plane ID (3 letters + digit + A/D, e.g. QFA8A): ");
            var pid = (Console.ReadLine() ?? "").Trim().ToUpper();
            while (!ValidationService.IsValidPlaneId(pid) || flights.Any(f => f.Plane.PlaneId == pid))
            {
                if (!ValidationService.IsValidPlaneId(pid)) Console.WriteLine("Invalid Plane ID format.");
                else Console.WriteLine("Plane ID already exists.");
                Console.Write("Plane ID: ");
                pid = (Console.ReadLine() ?? "").Trim().ToUpper();
            }

            Console.Write("City (Sydney/Melbourne/Rockhampton/Adelaide/Perth): ");
            var city = (Console.ReadLine() ?? "").Trim();

            Console.Write("Rows (1-10): ");
            int rows = ValidationService.ReadValidInt("", 1, 10);

            Console.Write("Seats per row (1-4): ");
            int seats = ValidationService.ReadValidInt("", 1, 4);

            Console.Write("Scheduled DateTime (yyyy-MM-dd HH:mm): ");
            DateTime dt;
            while (!DateTime.TryParse(Console.ReadLine(), out dt))
            {
                Console.WriteLine("Invalid datetime.");
                Console.Write("Scheduled DateTime: ");
            }

            // decide arrival/departure from last char of plane id A=arrival, D=departure
            var dir = pid.EndsWith("A") ? FlightDirection.Arrival : FlightDirection.Departure;
            var plane = new Plane(pid, rows, seats);
            var flight = new Flight(airline, fid, city, plane, dt, dir);
            flights.Add(flight);
            Console.WriteLine($"Flight {fid} added.");
        }

        public static void ShowAllFlights(List<Flight> flights)
        {
            if (!flights.Any()) { Console.WriteLine("No flights scheduled."); return; }
            foreach (var f in flights.OrderBy(f => f.ScheduledDateTime))
            {
                Console.WriteLine($"{f.FlightId} {f.Direction} {f.City} {f.ScheduledDateTime} Plane:{f.Plane.PlaneId} Status:{f.Status}");
            }
        }

        public static void UpdateFlightStatusInteractive(List<Flight> flights)
        {
            if (!flights.Any()) { Console.WriteLine("No flights scheduled."); return; }
            Console.Write("Flight ID to update: ");
            var fid = (Console.ReadLine() ?? "").Trim().ToUpper();
            var fl = flights.FirstOrDefault(x => x.FlightId == fid);
            if (fl == null) { Console.WriteLine("Flight not found."); return; }

            Console.Write("New Status (On Time/Delayed): ");
            var status = (Console.ReadLine() ?? "").Trim();
            fl.Status = status;
            if (status.Equals("Delayed", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("Delay minutes: ");
                int mins = ValidationService.ReadValidInt("", 0, 1000000);
                fl.ScheduledDateTime = fl.ScheduledDateTime.AddMinutes(mins);

                // adjust other flights using same plane id
                var related = flights.Where(x => x.Plane.PlaneId == fl.Plane.PlaneId && x != fl).ToList();
                foreach (var r in related)
                {
                    r.ScheduledDateTime = r.ScheduledDateTime.AddMinutes(mins);
                    Console.WriteLine($"Adjusted {r.FlightId} by {mins} minutes.");
                }

                Console.WriteLine($"Flight {fl.FlightId} delayed by {mins} minutes.");
            }
            else
            {
                Console.WriteLine($"Status updated to {status}");
            }
        }
    }
}
