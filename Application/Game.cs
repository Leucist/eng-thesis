using Application.Worlds;

namespace Application
{
    public class Game
    {
        // Game may stand for the initial game process, 
        // while World is a new game session when it's started
        //
        // i.e. World is for levels playthrough, when "Start new game" or "Load the game"
        // ..and Game is for World + Menu + Cutscenes, etc.
        
        private const uint FRAME_TIME = 16;   // 1000 ms => 1 fps

        private World? _world;
        private Timer _timer = new(FRAME_TIME);
        private bool _isRunning = false;
        
        private static Game? _instance = null;
        private static readonly object _lock = new();

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
        
        public void Start() {
            // [?] reset values

            // Start Main Menu
            _isRunning = true;
            _world = WorldFactory.InitialWorld;

            while (_isRunning) {
                _world = UpdateWorld();
            }
            // Update MM and handle further actions (e.g. load or create world and pass ctrl)
        }

        private World UpdateWorld() {
            // todo: May Implement switching levels with LinkedList<WorldProxy> (one-way linking) :D 

            while (_world!.IsAlive) {
                _timer.Reset();
                _world.Update();
                while (!_timer.FramePassed) System.Threading.Thread.Sleep(1);
            }

            throw new NotImplementedException();
            
            return _world;
        }
    }
}