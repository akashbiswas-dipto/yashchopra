using BrisbaneAirportSimple.Services;
using BrisbaneAirportSimple.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrisbaneAirportSimple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // load users from file
            var userlist = UserFileService.LoadUsers();
            var flightlist = new List<Flight>();
            var tickets = new List<Ticket>();

            // seed a default manager if none (optional)
            if (!userlist.Any(u => u is FlightManager))
            {
                var m = new FlightManager("Default Manager", 35, "manager@air.com", "0411000000", "Manager123", "STAFF001");
                userlist.Add(m);
                UserFileService.AppendUser(m);
            }

            Console.WriteLine("Welcome to Brisbane Airport Simple App");
            bool running = true;
            User? loggedIn = null;

            while (running)
            {
                // show dynamic menu
                Console.WriteLine();
                if (loggedIn == null)
                {
                    Console.WriteLine("1. Register");
                    Console.WriteLine("2. Login");
                    Console.WriteLine("3. Exit");
                    Console.Write("Choose: ");
                    var c = Console.ReadLine();

                    if (c == "1") RegisterUser(userlist);
                    else if (c == "2") loggedIn = LoginUser(userlist);
                    else if (c == "3") running = false;
                    else Console.WriteLine("Invalid choice");
                }
                else if (loggedIn is FlightManager)
                {
                    Console.WriteLine($"Logged in as Flight Manager: {loggedIn.Name}");
                    Console.WriteLine("1. View Flights");
                    Console.WriteLine("2. Add Flight");
                    Console.WriteLine("3. Update Flight Status (delay)");
                    Console.WriteLine("4. View Profile / Change Password");
                    Console.WriteLine("5. Logout");
                    Console.Write("Choose: ");
                    var c = Console.ReadLine();
                    if (c == "1") FlightService.ShowAllFlights(flightlist);
                    else if (c == "2") FlightService.AddFlightInteractive(flightlist);
                    else if (c == "3") FlightService.UpdateFlightStatusInteractive(flightlist);
                    else if (c == "4") ProfileMenu(loggedIn, userlist);
                    else if (c == "5") { loggedIn = null; Console.WriteLine("Logged out."); }
                    else Console.WriteLine("Invalid choice");
                }
                else // Traveller or FrequentFlyer
                {
                    Console.WriteLine($"Logged in as: {loggedIn.Name} ({loggedIn.GetType().Name})");
                    Console.WriteLine("1. View Flights");
                    Console.WriteLine("2. Book a Flight");
                    Console.WriteLine("3. View My Tickets");
                    Console.WriteLine("4. View Profile / Change Password");
                    Console.WriteLine("5. Logout");
                    Console.Write("Choose: ");
                    var c = Console.ReadLine();
                    if (c == "1") FlightService.ShowAllFlights(flightlist);
                    else if (c == "2")
                    {
                        var t = BookingService.BookFlightInteractive(loggedIn, flightlist, tickets);
                        if (t != null) tickets.Add(t);
                    }
                    else if (c == "3") BookingService.ShowUserTickets(loggedIn, tickets);
                    else if (c == "4") ProfileMenu(loggedIn, userlist);
                    else if (c == "5") { loggedIn = null; Console.WriteLine("Logged out."); }
                    else Console.WriteLine("Invalid choice");
                }
            }

            Console.WriteLine("Goodbye!");
        }

        static void RegisterUser(List<User> userlist)
        {
            Console.WriteLine("Register a new user");
            Console.WriteLine("Select type: 1=Traveller 2=FrequentFlyer 3=FlightManager");
            var t = Console.ReadLine();

            string name = ValidationService.ReadValidName("Name: ");
            int age = ValidationService.ReadValidInt("Age: ", 0, 99);
            string email = ValidationService.ReadValidEmailUnique("Email: ", userlist);
            string mobile = ValidationService.ReadValidMobile("Mobile: ");
            string pw = ValidationService.ReadValidPassword("Password: ");

            if (t == "1")
            {
                var tr = new Traveller(name, age, email, mobile, pw);
                userlist.Add(tr);
                UserFileService.AppendUser(tr);
                Console.WriteLine($"Traveller {name} registered.");
            }
            else if (t == "2")
            {
                int ffnum = ValidationService.ReadValidInt("Frequent Flyer Number (100000-999999): ", 100000, 999999);
                int pts = ValidationService.ReadValidInt("Starting points (0-1000000): ", 0, 1000000);
                var ff = new FrequentFlyer(name, age, email, mobile, pw, ffnum, pts);
                userlist.Add(ff);
                UserFileService.AppendUser(ff);
                Console.WriteLine($"FrequentFlyer {name} registered.");
            }
            else if (t == "3")
            {
                string staff = ValidationService.ReadNonEmpty("Staff ID: ");
                var fm = new FlightManager(name, age, email, mobile, pw, staff);
                userlist.Add(fm);
                UserFileService.AppendUser(fm);
                Console.WriteLine($"FlightManager {name} registered.");
            }
            else
            {
                Console.WriteLine("Invalid type. Registration cancelled.");
            }
        }

        static User? LoginUser(List<User> userlist)
        {
            // reload users from file to ensure latest
            userlist.Clear();
            userlist.AddRange(UserFileService.LoadUsers());

            Console.Write("Email: ");
            var e = Console.ReadLine() ?? "";
            Console.Write("Password: ");
            var p = Console.ReadLine() ?? "";

            var found = userlist.FirstOrDefault(u => u.Email.Equals(e, StringComparison.OrdinalIgnoreCase) && u.Password == p);
            if (found == null)
            {
                Console.WriteLine("Login failed - wrong email or password.");
                return null;
            }
            Console.WriteLine($"Welcome {found.Name}!");
            return found;
        }

        static void ProfileMenu(User user, List<User> userlist)
        {
            Console.WriteLine("--- Profile ---");
            Console.WriteLine(user.GetDetails());
            if (user is FrequentFlyer ff)
            {
                Console.WriteLine($"Frequent Flyer No: {ff.FFNumber}, Points: {ff.Points}");
            }
            if (user is FlightManager fm)
            {
                Console.WriteLine($"Staff ID: {fm.StaffId}");
            }

            Console.Write("Change password? (y/n): ");
            var ans = Console.ReadLine();
            if (ans != null && ans.Trim().ToLower() == "y")
            {
                string npw = ValidationService.ReadValidPassword("New password: ");
                user.Password = npw;
                // persist change to file by rewriting all users
                UserFileService.SaveAllUsers(userlist);
                Console.WriteLine("Password changed and saved.");
            }
        }
    }
}
