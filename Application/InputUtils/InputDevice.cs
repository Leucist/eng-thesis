using SFML.Graphics;

namespace Application.InputUtils
{
    public abstract class InputDevice
    {
        // protected readonly required Dictionary<,Input> _bindings;

        public abstract void Subscribe(RenderWindow window);
    }
}