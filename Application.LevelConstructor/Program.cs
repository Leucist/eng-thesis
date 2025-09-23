using Application;
using SFML.Graphics;
using SFML.Window;

namespace Application.LevelConstructor {
    public static class Program {
        private const string TITLE = "Level Constructor";

        public static void Main(string[] args) {
            RenderWindow window = new(new VideoMode(1920, 1080), TITLE, Styles.Fullscreen);

            while (window.IsOpen) {
                // do smth (or not)
            }
        }
    }
}
