using Application.AppMath;
using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class MovementSystem : System
    {
        // TODO: Depending on the future usage: to be changed to utilizing time between frames as parameter 
        private const int DELTA_TIME = 1;

        public MovementSystem(EntityManager entityManager) : base(entityManager) {
            List<ComponentType> requiredComponents = [
                ComponentType.Transform,
                ComponentType.Physics
            ];

            _requiredComponents.AddRange(requiredComponents);
        }

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