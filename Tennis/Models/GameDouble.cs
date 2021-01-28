namespace Tennis.Models
{
    public class GameDouble : Game
    {
        public (Player p1, Player p2) Team1 { get; set; }
        public (Player p3, Player p4) Team2 { get; set; }

        public override string ToString()
        {
            return $"{Team1.p1.Name} / {Team1.p2.Name} vs {Team2.p3.Name} / {Team2.p4.Name}";
        }
    }
}