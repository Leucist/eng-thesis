using SFML.Graphics;
using SFML.Window;

namespace Application.InputUtils
{
    public class KeyboardInputDevice : InputDevice
    {
        private Dictionary<Keyboard.Key, Input> _keyInputBindings = new(){
            {Keyboard.Key.Left,     Input.Left},
            {Keyboard.Key.A,        Input.Left},
            {Keyboard.Key.Right,    Input.Right},
            {Keyboard.Key.D,        Input.Right},
            {Keyboard.Key.Up,       Input.Up},
            {Keyboard.Key.W,        Input.Up},
            {Keyboard.Key.Down,     Input.Down},
            {Keyboard.Key.S,        Input.Down},
            {Keyboard.Key.Space,    Input.RisingMovement},
            {Keyboard.Key.Enter,    Input.ShortAction},
        };

        public override void Subscribe(RenderWindow window) {
            window.KeyPressed   += OnKeyPressed;
            window.KeyReleased  += OnKeyReleased;
        }


        // - Not particularly beatiful methods, but as is for now >

        private Input? ExtractInput(Keyboard.Key key) {
            if (_keyInputBindings.TryGetValue(key, out Input input)) {
                return input;
            }
            return null;
        }

        public void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            Input? input = ExtractInput(e.Code);
            if (input is not null) _inputThisFrame.Add((Input)input);
        }
        
        public void OnKeyReleased(object? sender, KeyEventArgs e)
        {
            Input? input = ExtractInput(e.Code);
            if (input is not null) _releasedThisFrame.Add((Input)input);
        }
    }
}