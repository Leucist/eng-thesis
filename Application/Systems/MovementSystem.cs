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
        private const int DELTA_TIME = 1;

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            TransformComponent  transformComponent  = (TransformComponent)  entityComponents[ComponentType.Transform];
            PhysicsComponent    physicsComponent    = (PhysicsComponent)    entityComponents[ComponentType.Physics];

            // Perform the system logic
            OffsetEntry movementOffset = physicsComponent.GetMovementOffset(DELTA_TIME);
            
            // todo: TEMP [2] buffer var to save the direction before fall
            int prevDirection = 0;
            if (physicsComponent.IsFalling) prevDirection = transformComponent.Direction;

            transformComponent.Move(movementOffset.X, -movementOffset.Y);

            // todo: TEMP [2]
            if (physicsComponent.IsFalling) transformComponent.Move(0.01f * prevDirection, 0);

            // TODO: TEMP [mv1] movement flag
            transformComponent.HasMoved = movementOffset.X != 0 || movementOffset.Y != 0;
        }
    }
}