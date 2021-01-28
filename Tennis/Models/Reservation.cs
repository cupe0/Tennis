using System;
using System.Collections.Generic;

namespace Tennis.Models
{
    public class Reservation
    {
        public Queue<Player> Players { get; set; }
        public WeekDay WeekDayEnum { get; set; }
        public DateTime Date { get; set; }
        public int NumberSingleGames { get; set; }
        public int NumberDoubleGames { get; set; }
        
        public int NumberOfPlayers { get; set; }

        public List<Game> Games { get; set; }

        public Reservation()
        {
            Games = new List<Game>();
        }
    }
}