namespace BrisbaneAirportSimple.Models
{
    public class Ticket
    {
        public User User;
        public Flight Flight;
        public string SeatCode;

        public Ticket(User user, Flight flight, string seat)
        {
            User = user;
            Flight = flight;
            SeatCode = seat;
        }
    }
}
