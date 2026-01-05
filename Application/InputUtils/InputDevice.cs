using SFML.Graphics;

namespace Application.InputUtils
{
    public abstract class InputDevice
    {
        protected HashSet<Input> _inputThisFrame    = [];
        protected HashSet<Input> _releasedThisFrame = [];

        public abstract void Subscribe(RenderWindow window);

        public virtual Input[] GetInput() {
            // // Fill the resulting array
            // Input[] input = new Input[_inputThisFrame.Count];
            // _inputThisFrame.CopyTo(input);
            // // Update the input queue
            // _inputThisFrame.ExceptWith(_releasedThisFrame);
            return [.. _inputThisFrame];
        }
    }
}