using Application.Components;
using Application.Entities;
using Application.InputUtils;
using Application.GraphicsUtils;
// todo Temp [2]
using static Application.AppMath.MathConstants;

namespace Application.Systems
{
    public class InputSystem : ASystem
    {
        public required InputDevice Device;

        // todo Temp? [1]
        private InputComponent _playerInputC;
        private PhysicsComponent _playerPhysicsC;
        // private CombatComponent _playerCombatC;
        // todo Temp [2] - START
        private delegate void ActionDelegate();
        private Dictionary<Input, ActionDelegate> _playerBindings;
        // todo Temp [2] - END

        public InputSystem(EntityManager entityManager)
            : base(
                entityManager,
                [
                    ComponentType.Input,
                    ComponentType.Physics,
                    // ComponentType.Combat,
                ]
            )  
        {
            // todo Temp [3] // To make it easier in init for now
            Device = new KeyboardInputDevice();
            Device.Subscribe(WindowManager.Window);

            // todo Temp? [1]
            SetPlayer();
            // todo Temp [2] - START
            _playerBindings = new() {
                {Input.Left, MovePlayerLeft},
                {Input.Right, MovePlayerRight}
            };
            // todo Temp [2] - END
        }

        private void SetPlayer() {
            List<Component> player = _entityManager.GetAllComponentBundlesWith(_requiredComponents)[0];
            
            foreach (var component in player) {
                switch (component.Type) {
                    case ComponentType.Input:
                        _playerInputC = (InputComponent) component;
                        break;
                    case ComponentType.Physics:
                        _playerPhysicsC = (PhysicsComponent) component;
                        break;
                }
            }
        }

        // * Overriding the base ASystem.Update
        // * As only one entity is expected to be steerable via user input FOR NOW!
        // * Despite the Input module is being designed for high scalability
        public override void Update() {
            Input[] currentFrameInput = Device.GetInput();
            // Iterate through each Input provided
            foreach (var input in currentFrameInput) {
                // todo Temp [2] - START
                if (_playerBindings.TryGetValue(input, out var action))
                {
                    action.Invoke(); // call delegate
                }
                // todo Temp [2] - END
                // _playerCommandC.Add(_playerInputC.ProcessInput(input));
            }
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents)
        {
            throw new NotImplementedException();
        }

        // protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
        //     InputComponent      inputComponent      = (InputComponent)      entityComponents[ComponentType.Input];
        //     CommandComponent    commandComponent    = (CommandComponent)    entityComponents[ComponentType.Command];

        //     // Iterate through each Input provided
        //     foreach (var input in currentFrameInput) {
        //         commandComponent.Add(inputComponent.ProcessInput(input));
        //     }
        // }

        // todo Temp [2] - Delegates below:
        private void MovePlayer(float direction, int magnitude=200) {
            // * Debug log :D
            // Console.WriteLine($"- PLAYER\tFa.Value: {_playerPhysicsC.AppliedForce.Value}\tFa.Angle: {_playerPhysicsC.AppliedForce.Angle}");
            _playerPhysicsC.AddAppliedForce(new(magnitude, direction));
        }
        private void MovePlayerLeft() => MovePlayer(RadiansLeftDirection);
        private void MovePlayerRight() => MovePlayer(RadiansRightDirection);
        // private void MovePlayerUp() => MovePlayer(RadiansUpDirection);
        // private void MovePlayerDown() => MovePlayer(RadiansDownDirection);
    }
}