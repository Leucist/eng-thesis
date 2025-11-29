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
            transformComponent.Move(movementOffset.X, -movementOffset.Y);

            // TODO: TEMP [mv1] movement flag
            transformComponent.HasMoved = (movementOffset.X != 0 || movementOffset.Y != 0) ? true : false;
        }
    }
}