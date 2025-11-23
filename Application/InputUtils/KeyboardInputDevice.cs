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
        };

        public override void Subscribe(RenderWindow window) {
            window.KeyPressed   += (object? sender, KeyEventArgs e) => OnKeyPressed (_keyInputBindings[e.Code]);
            window.KeyReleased  += (object? sender, KeyEventArgs e) => OnKeyReleased(_keyInputBindings[e.Code]);
        }

        public void OnKeyPressed(Input i)
        {
            _inputThisFrame.Add(i);
        }
        
        public void OnKeyReleased(Input i)
        {
            _releasedThisFrame.Add(i);
        }
    }
}