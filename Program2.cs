using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BrisbaneAirportApp
{
    // User types
    enum UserType
    {
        Traveller,
        FrequentFlyer,
        FlightManager
    }

    // Base user class
    class User
    {
        public string Name;
        public int Age;
        public string Email;
        public string Mobile;
        public string Password;
        public UserType Type;

        public User(string name, int age, string email, string mobile, string password, UserType type)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = mobile;
            Password = password;
            Type = type;
        }

        public virtual string ToFileString()
        {
            return $"{Name}|{Age}|{Email}|{Mobile}|{Password}|{Type}";
        }
    }

    // Frequent flyer inherits user
    class FrequentFlyer : User
    {
        public int FrequentFlyerNumber;
        public int Points;

        public FrequentFlyer(string name, int age, string email, string mobile, string password, int ffNum, int points)
            : base(name, age, email, mobile, password, UserType.FrequentFlyer)
        {
            FrequentFlyerNumber = ffNum;
            Points = points;
        }

        public override string ToFileString()
        {
            return $"{Name}|{Age}|{Email}|{Mobile}|{Password}|{Type}|{FrequentFlyerNumber}|{Points}";
        }
    }

    // Flight Manager inherits user
    class FlightManager : User
    {
        public string StaffID;

        public FlightManager(string name, int age, string email, string mobile, string password, string staffId)
            : base(name, age, email, mobile, password, UserType.FlightManager)
        {
            StaffID = staffId;
        }

        public override string ToFileString()
        {
            return $"{Name}|{Age}|{Email}|{Mobile}|{Password}|{Type}|{StaffID}";
        }
    }

    // Simple Seat and Plane classes for later extension
    class Seat
    {
        public string SeatNumber;
        public bool IsBooked;
        public Seat(string seatNumber)
        {
            SeatNumber = seatNumber;
            IsBooked = false;
        }
    }

    class Plane
    {
        public string PlaneId;
        public List<Seat> Seats = new List<Seat>();
        public Plane(string id, int rows, int seatsPerRow)
        {
            PlaneId = id;
            for (int r = 1; r <= rows; r++)
            {
                for (int s = 0; s < seatsPerRow; s++)
                {
                    char seatLetter = (char)('A' + s);
                    Seats.Add(new Seat($"{r}{seatLetter}"));
                }
            }
        }
    }

    internal class Program
    {
        static string filePath = "users.txt";
        static User? loggedInUser = null;

        static void Main(string[] args)
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("=  Welcome to Brisbane Domestic Airport  =");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            if (!File.Exists(filePath))
                File.Create(filePath).Close();

            bool exit = false;
            while (!exit)
            {
                if (loggedInUser == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Please make a choice from the menu below:");
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register");
                    Console.WriteLine("3. Exit");
                    Console.Write("Choice: ");
                    string choice = Console.ReadLine() ?? "";

                    if (choice == "1")
                        Login();
                    else if (choice == "2")
                        Register();
                    else if (choice == "3")
                    {
                        Console.WriteLine("Thank you. Safe travels!");
                        exit = true;
                    }
                    else
                        Console.WriteLine("Invalid choice, please try again.");
                }
                else
                {
                    ShowUserMenu();
                }
            }
        }

        // Login
        static void Login()
        {
            Console.Write("Email: ");
            string email = (Console.ReadLine() ?? "").Trim();
            Console.Write("Password: ");
            string password = (Console.ReadLine() ?? "").Trim();

            var users = File.ReadAllLines(filePath).ToList();
            foreach (var line in users)
            {
                var parts = line.Split('|');
                if (parts.Length >= 6 && parts[2] == email && parts[4] == password)
                {
                    string name = parts[0];
                    int age = int.Parse(parts[1]);
                    string mobile = parts[3];
                    string typeStr = parts[5];
                    Enum.TryParse(typeStr, out UserType type);

                    if (type == UserType.FrequentFlyer && parts.Length >= 8)
                        loggedInUser = new FrequentFlyer(name, age, email, mobile, password, int.Parse(parts[6]), int.Parse(parts[7]));
                    else if (type == UserType.FlightManager && parts.Length >= 7)
                        loggedInUser = new FlightManager(name, age, email, mobile, password, parts[6]);
                    else
                        loggedInUser = new User(name, age, email, mobile, password, type);

                    Console.WriteLine($"Welcome {loggedInUser.Name}!");
                    return;
                }
            }
            Console.WriteLine("Login failed. Please check your email or password.");
        }

        // Registration
        static void Register()
        {
            Console.WriteLine("Register as:");
            Console.WriteLine("1. Traveller");
            Console.WriteLine("2. Frequent Flyer");
            Console.WriteLine("3. Flight Manager");
            Console.Write("Choice: ");
            string typeChoice = Console.ReadLine() ?? "1";

            Console.Write("Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("Age: ");
            int age = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Mobile: ");
            string mobile = Console.ReadLine() ?? "";
            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";

            // Ensure unique email
            if (File.ReadAllLines(filePath).Any(l => l.Contains(email)))
            {
                Console.WriteLine("This email is already registered.");
                return;
            }

            Console.Write("Password: ");
            string password = Console.ReadLine() ?? "";

            User newUser;

            if (typeChoice == "2")
            {
                Console.Write("Frequent Flyer Number: ");
                int ffnum = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Points: ");
                int points = int.Parse(Console.ReadLine() ?? "0");
                newUser = new FrequentFlyer(name, age, email, mobile, password, ffnum, points);
            }
            else if (typeChoice == "3")
            {
                Console.Write("Staff ID: ");
                string staffId = Console.ReadLine() ?? "";
                newUser = new FlightManager(name, age, email, mobile, password, staffId);
            }
            else
            {
                newUser = new User(name, age, email, mobile, password, UserType.Traveller);
            }

            File.AppendAllText(filePath, newUser.ToFileString() + Environment.NewLine);
            Console.WriteLine($"Registration successful! Welcome, {name}.");
        }

        // Show user menu after login
        static void ShowUserMenu()
        {
            Console.WriteLine();
            Console.WriteLine($"Welcome back, {loggedInUser!.Name} ({loggedInUser.Type})");
            Console.WriteLine("1. View My Details");
            Console.WriteLine("2. Change Password");
            Console.WriteLine("3. Logout");
            Console.Write("Choice: ");
            string choice = Console.ReadLine() ?? "";

            if (choice == "1")
                ViewDetails();
            else if (choice == "2")
                ChangePassword();
            else if (choice == "3")
            {
                loggedInUser = null;
                Console.WriteLine("You have logged out successfully.");
            }
            else
                Console.WriteLine("Invalid choice.");
        }

        static void ViewDetails()
        {
            Console.WriteLine($"Name: {loggedInUser!.Name}");
            Console.WriteLine($"Age: {loggedInUser.Age}");
            Console.WriteLine($"Email: {loggedInUser.Email}");
            Console.WriteLine($"Mobile: {loggedInUser.Mobile}");
            if (loggedInUser is FrequentFlyer ff)
                Console.WriteLine($"Frequent Flyer #: {ff.FrequentFlyerNumber}, Points: {ff.Points}");
            if (loggedInUser is FlightManager fm)
                Console.WriteLine($"Staff ID: {fm.StaffID}");
        }

        static void ChangePassword()
        {
            Console.Write("Enter current password: ");
            string oldPass = Console.ReadLine() ?? "";
            if (oldPass != loggedInUser!.Password)
            {
                Console.WriteLine("Incorrect current password.");
                return;
            }
            Console.Write("Enter new password: ");
            string newPass = Console.ReadLine() ?? "";
            loggedInUser.Password = newPass;

            // Update file
            var lines = File.ReadAllLines(filePath).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains(loggedInUser.Email))
                    lines[i] = loggedInUser.ToFileString();
            }
            File.WriteAllLines(filePath, lines);

            Console.WriteLine("Password changed successfully.");
        }
    }
}
