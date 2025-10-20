using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BrisbaneAirportSimple.Models;

namespace BrisbaneAirportSimple.Services
{
    public static class ValidationService
    {
        // Name: letters, spaces, apostrophes, hyphens
        private static Regex nameRx = new Regex(@"^[A-Za-z '\-]+$");

        public static string ReadValidName(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = (Console.ReadLine() ?? "").Trim();
                if (!string.IsNullOrEmpty(s) && nameRx.IsMatch(s)) return s;
                Console.WriteLine("Invalid name. Use letters, spaces, apostrophes and hyphens only.");
            }
        }

        public static int ReadValidInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (int.TryParse(s, out int v) && v >= min && v <= max) return v;
                Console.WriteLine($"Enter a number between {min} and {max}.");
            }
        }

        public static string ReadValidEmailUnique(string prompt, List<User> users)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = (Console.ReadLine() ?? "").Trim();
                if (!IsValidEmail(s)) { Console.WriteLine("Invalid email."); continue; }
                if (users.Any(u => u.Email.Equals(s, StringComparison.OrdinalIgnoreCase))) { Console.WriteLine("Email already in use."); continue; }
                return s;
            }
        }

        public static string ReadValidMobile(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = (Console.ReadLine() ?? "").Trim();
                if (s.Length == 10 && s.All(char.IsDigit) && s.StartsWith("0")) return s;
                Console.WriteLine("Invalid mobile. Must be exactly 10 digits and start with 0.");
            }
        }

        public static string ReadValidPassword(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var pw = Console.ReadLine() ?? "";
                bool okLen = pw.Length >= 8;
                bool hasDigit = pw.Any(char.IsDigit);
                bool hasLower = pw.Any(char.IsLower);
                bool hasUpper = pw.Any(char.IsUpper);
                if (okLen && hasDigit && hasLower && hasUpper) return pw;
                Console.WriteLine("Password must be at least 8 chars and include uppercase, lowercase and a digit.");
            }
        }

        public static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = (Console.ReadLine() ?? "").Trim();
                if (!string.IsNullOrEmpty(s)) return s;
                Console.WriteLine("Input cannot be empty.");
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var parts = email.Split('@');
            return parts.Length == 2 && parts[0].Length > 0 && parts[1].Length > 0;
        }

        // Flight id (3 letters + 3 digits)
        public static bool IsValidFlightId(string id)
        {
            return Regex.IsMatch(id ?? "", @"^[A-Z]{3}\d{3}$");
        }

        public static bool IsValidPlaneId(string id)
        {
            return Regex.IsMatch(id ?? "", @"^[A-Z]{3}\d[AD]$");
        }

        public static bool IsValidSeat(string seat)
        {
            return Regex.IsMatch(seat ?? "", @"^([1-9]|10)[A-D]$");
        }
    }
}
