using Application.AppMath;
using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class MovementSystem(EntityManager entityManager)
        : ASystem(
            entityManager,
            [
                ComponentType.Transform,
                ComponentType.Physics
            ]
        )
    {
        // TODO: Depending on the future usage: to be changed to utilizing time between frames as parameter 
        private const int DELTA_TIME = 1;

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            TransformComponent  transformComponent  = (TransformComponent)  entityComponents[ComponentType.Transform];
            PhysicsComponent    physicsComponent    = (PhysicsComponent)    entityComponents[ComponentType.Physics];

            // Perform the system logic
            OffsetEntry movementOffset = physicsComponent.GetMovementOffset(DELTA_TIME);
            transformComponent.Move(movementOffset.X, -movementOffset.Y);
        }
    }
}