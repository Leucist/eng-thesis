namespace Application 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Game game = Game.Instance;
            game.Start();
        }
    }
}