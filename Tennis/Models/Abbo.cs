using System;
using System.Collections.Generic;

namespace Tennis.Models
{
    public class Abbo
    {
        
        public static List<Abbo> AbboList = new() { };
        
        public Queue<Player> Group { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Reservation> Reservations { get; set; }
        public List<DateTime> ReservationDates { get; set; }

        public Abbo()
        {
            Reservations = new List<Reservation>();
            Group = new Queue<Player>();
            AbboList.Add(this);
        }
        
        //Todo: Not so good - Refactor Abbo Class to a Form Class
        public string GroupInput { get; set; }
        
        public string ReservationDayInput { get; set; }

        public string NumberOfSingleGamesInput { get; set; }
        
        public string NumberOfDoubleGamesInput { get; set; }
        
    }
}