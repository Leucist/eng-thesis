using SFML.Graphics;

namespace Application.InputUtils
{
    public abstract class InputDevice
    {
        protected HashSet<Input> _input = [];

        public abstract void Subscribe(RenderWindow window);

        public virtual Input[] GetInput() {
            return [.. _input];
        }
    }
}