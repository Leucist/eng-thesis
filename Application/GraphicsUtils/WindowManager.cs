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
            // _renderWindow       = new(videoMode, TITLE, Styles.Fullscreen);
            
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
    }
}