namespace Application
{
    public class Game
    {
        // Game may stand for the initial game process, 
        // while World is a new game session when it's started
        //
        // i.e. World is for levels playthrough, when "Start new game" or "Load the game"
        // ..and Game is for World + Menu + Cutscenes, etc.

        private static Game? _instance = null;
        private static object _lock = new();

        public static Game Instance {
            get {
                if (_instance == null) {
                    lock (_lock) {
                        _instance = new Game();
                    }
                }
                return _instance;
            }
        }
        
        public void Start() {}
    }
}