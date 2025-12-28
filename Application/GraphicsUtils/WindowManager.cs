using SFML.Graphics;
using SFML.Window;

namespace Application.GraphicsUtils
{
    public class WindowManager
    {
        // * Window Instance *
        private static RenderWindow _renderWindow;
        public  static RenderWindow Window => _renderWindow;
        
        // static constructor
        static WindowManager()
        {
            // * WINDOW mode *
            VideoMode videoMode = new(AppConstants.WIDTH, AppConstants.HEIGHT);
            _renderWindow       = new(videoMode, AppConstants.TITLE);
            // * FULL SCREEN mode *
            // VideoMode videoMode = VideoMode.DesktopMode;
            // _renderWindow       = new(videoMode, AppConstants.TITLE, Styles.Fullscreen);
            
            // Registers essential events
            SetHandlers();
        }
        
        private static void SetHandlers() {
            _renderWindow.Closed += OnWindowClosed;
        }

        private static void OnWindowClosed(object? sender, EventArgs e)
        {
            // Close the window and exit when the close button is clicked
            _renderWindow.Close();
            Environment.Exit(0);
        }
        
        public static void DispatchEvents()
        {
            _renderWindow.DispatchEvents();
        }


        // todo: Temp?
        public static void GameOver(string msg) {
            var font = new Font(Path.Combine(Pathfinder.GetSourceFolder(), "cinzel-font.ttf"));
            var text = new Text(msg, font, 50);
            text.FillColor = Color.White;
            // Get window dimensions
            float windowWidth = _renderWindow.Size.X;
            float windowHeight = _renderWindow.Size.Y;

            // Get text dimensions
            FloatRect textRect = text.GetLocalBounds(); // Get the bounding box of the text

            // Center the text
            text.Origin = new SFML.System.Vector2f(textRect.Width / 2, textRect.Height / 2);
            text.Position = new SFML.System.Vector2f(windowWidth / 2, windowHeight / 2);

            RectangleShape overlay = new RectangleShape(new SFML.System.Vector2f(_renderWindow.Size.X, _renderWindow.Size.Y));
            overlay.FillColor = new Color(0, 0, 0, 190);

            // _renderWindow.Clear(Color.Black);
            _renderWindow.Draw(overlay);
            _renderWindow.Draw(text);

            _renderWindow.Display();
        } 
    }
}