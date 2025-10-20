using System;
using System.Collections.Generic;
using System.IO;
using BrisbaneAirportSimple.Models;

namespace BrisbaneAirportSimple.Services
{
    public static class UserFileService
    {
        private static string filePath = "users.txt";

        // append one user to file
        public static void AppendUser(User u)
        {
            try
            {
                File.AppendAllLines(filePath, new[] { u.ToTxtLine() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
            }
        }

        // load all users from file
        public static List<User> LoadUsers()
        {
            var list = new List<User>();
            try
            {
                if (!File.Exists(filePath)) return list;
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    try
                    {
                        var u = User.FromTxtLine(line);
                        list.Add(u);
                    }
                    catch
                    {
                        // ignore bad lines
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
            return list;
        }

        // save all users (overwrite file)
        public static void SaveAllUsers(List<User> users)
        {
            try
            {
                var lines = new List<string>();
                foreach (var u in users) lines.Add(u.ToTxtLine());
                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing users file: {ex.Message}");
            }
        }
    }
}
