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

            // generate seats
            for (int r = 1; r <= rows; r++)
            {
                for (int s = 0; s < seatsPerRow; s++)
                {
                    char letter = (char)('A' + s);
                    Seats.Add(new Seat($"{r}{letter}"));
                }
            }
        }
    }
}
