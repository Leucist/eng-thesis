using Application.AppMath;
using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class MovementSystem(EntityManager entityManager)
        : System(
            entityManager,
            [
                ComponentType.Transform,
                ComponentType.Physics
            ]
        )
    {
        // TODO: Depending on the future usage: to be changed to utilizing time between frames as parameter 
        private const int DELTA_TIME = 1;

        public override void Update() {
            var componentBundles = GatherComponents();

            foreach (var componentBundle in componentBundles) {
                TransformComponent  transformComponent  = (TransformComponent)  componentBundle[0];
                PhysicsComponent    physicsComponent    = (PhysicsComponent)    componentBundle[1];

                OffsetEntry movementOffset = physicsComponent.GetMovementOffset(DELTA_TIME);
                transformComponent.Move(movementOffset.X, -movementOffset.Y);
            }
        }
    }
}