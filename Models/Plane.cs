using System.Collections.Generic;

namespace BrisbaneAirportSimple.Models
{
    public class Plane
    {
        public string PlaneId;
        public int Rows;
        public int SeatsPerRow;
        public List<Seat> Seats = new List<Seat>();

        public Plane(string id, int rows, int seatsPerRow)
        {
            PlaneId = id;
            Rows = rows;
            SeatsPerRow = seatsPerRow;
            GenerateSeats();
        }

        void GenerateSeats()
        {
            Seats.Clear();
            for (int r = 1; r <= Rows; r++)
            {
                for (int s = 0; s < SeatsPerRow; s++)
                {
                    char letter = (char)('A' + s);
                    Seats.Add(new Seat($"{r}{letter}"));
                }
            }
        }

        public Seat? FindSeat(string code)
        {
            return Seats.Find(s => s.Code == code);
        }
    }
}
