using System;

namespace BrisbaneAirportSimple.Models
{
    // base user class
    public class User
    {
        public string Name;
        public int Age;
        public string Email;
        public string Mobile;
        public string Password;

        public User(string name, int age, string email, string mobile, string password)
        {
            Name = name;
            Age = age;
            Email = email;
            Mobile = mobile;
            Password = password;
        }

        public virtual string ToTxtLine()
        {
            // default user serialization
            return $"User|{Name}|{Age}|{Email}|{Mobile}|{Password}";
        }

        public static User FromTxtLine(string line)
        {
            // parse the plain text format
            var parts = line.Split('|');
            var type = parts[0];
            if (type == "Traveller")
            {
                return new Traveller(parts[1], int.Parse(parts[2]), parts[3], parts[4], parts[5]);
            }
            else if (type == "FrequentFlyer")
            {
                return new FrequentFlyer(parts[1], int.Parse(parts[2]), parts[3], parts[4], parts[5], int.Parse(parts[6]), int.Parse(parts[7]));
            }
            else if (type == "FlightManager")
            {
                return new FlightManager(parts[1], int.Parse(parts[2]), parts[3], parts[4], parts[5], parts[6]);
            }
            else
            {
                return new User(parts[1], int.Parse(parts[2]), parts[3], parts[4], parts[5]);
            }
        }

        public virtual string GetDetails()
        {
            return $"Name: {Name}\nAge: {Age}\nEmail: {Email}\nMobile: {Mobile}";
        }
    }

    public class Traveller : User
    {
        public Traveller(string name, int age, string email, string mobile, string password)
            : base(name, age, email, mobile, password) { }

        public override string ToTxtLine() => $"Traveller|{Name}|{Age}|{Email}|{Mobile}|{Password}";
    }

    public class FrequentFlyer : User
    {
        public int FFNumber;
        public int Points;

        public FrequentFlyer(string name, int age, string email, string mobile, string password, int ffNumber, int points)
            : base(name, age, email, mobile, password)
        {
            FFNumber = ffNumber;
            Points = points;
        }

        public override string ToTxtLine() => $"FrequentFlyer|{Name}|{Age}|{Email}|{Mobile}|{Password}|{FFNumber}|{Points}";

        public override string GetDetails()
        {
            return base.GetDetails() + $"\nFF Number: {FFNumber}\nPoints: {Points}";
        }
    }

    public class FlightManager : User
    {
        public string StaffId;

        public FlightManager(string name, int age, string email, string mobile, string password, string staffId)
            : base(name, age, email, mobile, password)
        {
            StaffId = staffId;
        }

        public override string ToTxtLine() => $"FlightManager|{Name}|{Age}|{Email}|{Mobile}|{Password}|{StaffId}";

        public override string GetDetails()
        {
            return base.GetDetails() + $"\nStaff ID: {StaffId}";
        }
    }
}
