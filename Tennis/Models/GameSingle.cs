namespace Tennis.Models
{
    public class GameSingle : Game
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        
        public override string ToString()
        {
            return $"{Player1.Name} vs {Player2.Name}";
        }
    }
}