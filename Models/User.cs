namespace BrisbaneAirportSimple.Models
{
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
    }

    public class Traveller : User
    {
        public Traveller(string name, int age, string email, string mobile, string password)
            : base(name, age, email, mobile, password) { }
    }

    public class FrequentFlyer : User
    {
        public string FFNumber;
        public int Points;

        public FrequentFlyer(string name, int age, string email, string mobile, string password, string ffnumber, int points)
            : base(name, age, email, mobile, password)
        {
            FFNumber = ffnumber;
            Points = points;
        }
    }

    public class FlightManager : User
    {
        public string StaffId;

        public FlightManager(string name, int age, string email, string mobile, string password, string staffid)
            : base(name, age, email, mobile, password)
        {
            StaffId = staffid;
        }
    }
}
