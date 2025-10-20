using System;
using System.Collections.Generic;
using System.Linq;
using BrisbaneAirportSimple.Models;
using BrisbaneAirportSimple.Services;

namespace BrisbaneAirportSimple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<User> userlist = new List<User>();
            List<Flight> flightlist = new List<Flight>();
            List<Ticket> tickets = new List<Ticket>();

            // Add some default users
            userlist.Add(new Traveller("Sam", 25, "sam@email.com", "0411000", "pass123"));
            userlist.Add(new FrequentFlyer("Lena", 30, "lena@email.com", "0411222", "1234", "FF001", 100));
            userlist.Add(new FlightManager("Maya", 35, "maya@air.com", "0411999", "mgrpass", "STAFF001"));

            bool run = true;
            User? loggedInUser = null;

            while (run)
            {
                Console.WriteLine("\n=== Brisbane Airport Menu ===");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. View Flights");
                Console.WriteLine("4. Book a Flight");
                Console.WriteLine("5. Show Tickets");
                Console.WriteLine("6. View Profile / Change Password");
                Console.WriteLine("7. Add Flight (Flight Manager)");
                Console.WriteLine("8. Update Flight Status (Flight Manager)");
                Console.WriteLine("9. Logout");
                Console.WriteLine("0. Exit");
                Console.Write("Choose option: ");
                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1": RegisterUser(userlist); break;
                    case "2": loggedInUser = LoginUser(userlist); break;
                    case "3": ShowAllFlights(flightlist); break;
                    case "4":
                        if (loggedInUser != null)
                        {
                            Ticket t = BookingService.BookFlightInteractive(loggedInUser, flightlist);
                            if (t != null) tickets.Add(t);
                        }
                        else Console.WriteLine("You must log in first!");
                        break;
                    case "5": ShowTickets(tickets); break;
                    case "6":
                        if (loggedInUser != null) ProfileMenu(loggedInUser);
                        else Console.WriteLine("You must log in first!");
                        break;
                    case "7":
                        if (loggedInUser is FlightManager) FlightManagerMenu(flightlist);
                        else Console.WriteLine("Only Flight Managers can add flights.");
                        break;
                    case "8":
                        if (loggedInUser is FlightManager) UpdateFlightStatus(flightlist);
                        else Console.WriteLine("Only Flight Managers can update flights.");
                        break;
                    case "9": loggedInUser = null; Console.WriteLine("Logged out successfully."); break;
                    case "0": run = false; break;
                    default: Console.WriteLine("Invalid option!"); break;
                }
            }
        }

        static void RegisterUser(List<User> userlist)
        {
            Console.Write("Name: "); string name = Console.ReadLine()!;
            Console.Write("Age: "); int age = int.Parse(Console.ReadLine()!);
            Console.Write("Email: "); string email = Console.ReadLine()!;
            Console.Write("Mobile: "); string mobile = Console.ReadLine()!;
            Console.Write("Password: "); string password = Console.ReadLine()!;
            Console.WriteLine("Select type: 1=Traveller, 2=Frequent Flyer, 3=Flight Manager");
            string type = Console.ReadLine()!;

            User newUser;
            if (type == "1") newUser = new Traveller(name, age, email, mobile, password);
            else if (type == "2")
            {
                Console.Write("FF Number: "); string ffnum = Console.ReadLine()!;
                Console.Write("Starting Points: "); int points = int.Parse(Console.ReadLine()!);
                newUser = new FrequentFlyer(name, age, email, mobile, password, ffnum, points);
            }
            else if (type == "3")
            {
                Console.Write("Staff ID: "); string staffid = Console.ReadLine()!;
                newUser = new FlightManager(name, age, email, mobile, password, staffid);
            }
            else
            {
                Console.WriteLine("Invalid type, creating Traveller by default.");
                newUser = new Traveller(name, age, email, mobile, password);
            }

            userlist.Add(newUser);
            Console.WriteLine($"User {newUser.Name} registered successfully!");
        }

        static User? LoginUser(List<User> userlist)
        {
            Console.Write("Email: "); string email = Console.ReadLine()!;
            Console.Write("Password: "); string pass = Console.ReadLine()!;
            var user = userlist.FirstOrDefault(u => u.Email == email && u.Password == pass);
            if (user != null) Console.WriteLine($"Welcome {user.Name}!");
            else Console.WriteLine("Login failed!");
            return user;
        }

        static void ShowAllFlights(List<Flight> flightlist)
        {
            Console.WriteLine("Flights:");
            foreach (var f in flightlist.OrderBy(f => f.ScheduledDateTime))
                Console.WriteLine($"{f.FlightCode} to {f.City} at {f.ScheduledDateTime}, PlaneID: {f.Plane.PlaneId}, Status: {f.Status}");
        }

        static void ShowTickets(List<Ticket> tickets)
        {
            Console.WriteLine("Tickets:");
            foreach (var t in tickets)
                Console.WriteLine($"{t.User.Name} booked {t.SeatCode} on {t.Flight.FlightCode} ({t.Flight.City})");
        }

        static void ProfileMenu(User user)
        {
            Console.WriteLine($"--- Profile for {user.Name} ---");
            Console.WriteLine($"Name: {user.Name}, Age: {user.Age}, Email: {user.Email}, Mobile: {user.Mobile}");
            if (user is FrequentFlyer ff) Console.WriteLine($"FF Number: {ff.FFNumber}, Points: {ff.Points}");
            if (user is FlightManager fm) Console.WriteLine($"Staff ID: {fm.StaffId}");
            Console.Write("Change password? (y/n): ");
            if (Console.ReadLine()!.ToLower() == "y")
            {
                Console.Write("New password: "); user.Password = Console.ReadLine()!;
                Console.WriteLine("Password updated!");
            }
        }

        static void FlightManagerMenu(List<Flight> flightlist)
        {
            Console.Write("Airline: "); string airline = Console.ReadLine()!;
            Console.Write("Flight Code: "); string code = Console.ReadLine()!;
            Console.Write("City: "); string city = Console.ReadLine()!;
            Console.Write("Plane ID: "); string planeId = Console.ReadLine()!;
            if (flightlist.Any(f => f.Plane.PlaneId == planeId)) { Console.WriteLine("Plane ID exists!"); return; }
            Console.Write("Rows: "); int rows = int.Parse(Console.ReadLine()!);
            Console.Write("Seats per row: "); int seats = int.Parse(Console.ReadLine()!);
            Plane plane = new Plane(planeId, rows, seats);
            Console.Write("Scheduled DateTime (yyyy-MM-dd HH:mm): "); DateTime dt = DateTime.Parse(Console.ReadLine()!);
            Flight flight = new Flight(airline, code, city, plane, dt);
            flightlist.Add(flight);
            Console.WriteLine("Flight added!");
        }

        static void UpdateFlightStatus(List<Flight> flightlist)
        {
            ShowAllFlights(flightlist);
            Console.Write("Flight code to update: "); string code = Console.ReadLine()!;
            Flight? f = flightlist.FirstOrDefault(x => x.FlightCode == code);
            if (f == null) { Console.WriteLine("Flight not found!"); return; }
            Console.Write("New Status (On Time / Delayed): "); string status = Console.ReadLine()!;
            f.Status = status;
            if (status.ToLower() == "delayed")
            {
                Console.Write("Delay duration in minutes: "); int delay = int.Parse(Console.ReadLine()!);
                f.ScheduledDateTime = f.ScheduledDateTime.AddMinutes(delay);
                Console.WriteLine($"Flight delayed by {delay} minutes!");
            }
        }
    }
}
