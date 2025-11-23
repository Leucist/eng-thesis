using Application.Components;
using Application.Entities;
using Application.InputUtils;

namespace Application.Systems
{
    public class InputSystem : ASystem
    {
        public required InputDevice Device;
        private Input[] currentFrameInput;

        // todo Temp? [1]
        private InputComponent _playerInputC;

        public InputSystem(EntityManager entityManager)
            : base(
                entityManager,
                [
                    ComponentType.Input,
                ]
            )  
        {
            // _device = device;
            currentFrameInput = [];

            // todo Temp? [1]
            SetPlayer();
        }

        private void SetPlayer() {
            List<Component> player = _entityManager.GetAllEntitiesWith(_requiredComponents)[0];
            
            foreach (var component in player) {
                switch (component.Type) {
                    case ComponentType.Input:
                        _playerInputC = (InputComponent) component;
                        break;
                    case ComponentType.Command:
                        commandComponent = (CommandComponent) component;
                        break;
                }
            }

            if (_playerInputC is null) throw new Exception("Player not found!");
        }

        // * Don't need this one if the field is public :P
        // public void SwitchDevice(InputDevice device) {
        //     Device = device;
        // }

        // * Overriding the base ASystem.Update
        // * As only one entity is expected to be steerable via user input FOR NOW!
        // * Despite the Input module is being designed for high scalability
        public override void Update() {
            // Iterate through each Input provided
            foreach (var input in currentFrameInput) {
                _playerCommandC.Add(_playerInputC.ProcessInput(input));
            }
        }

        // protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
        //     InputComponent      inputComponent      = (InputComponent)      entityComponents[ComponentType.Input];
        //     CommandComponent    commandComponent    = (CommandComponent)    entityComponents[ComponentType.Command];

        //     // Iterate through each Input provided
        //     foreach (var input in currentFrameInput) {
        //         commandComponent.Add(inputComponent.ProcessInput(input));
        //     }
        // }
    }
}