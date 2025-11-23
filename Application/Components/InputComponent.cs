using Application.InputUtils;

namespace Application.Components
{
    public class InputComponent() : Component(ComponentType.Input)
    {
        private readonly Dictionary<Input, ICommand> _bindings;

        public ICommand ProcessInput(Input input) => _bindings[input];
    }
}