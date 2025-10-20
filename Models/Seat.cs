namespace BrisbaneAirportSimple.Models
{
    public class Seat
    {
        public string Code;
        public bool IsBooked = false;

        public Seat(string code)
        {
            Code = code;
        }
    }
}
