using Application.Components;
using Application.Entities;
using SFML.Graphics;

namespace Application.Systems
{
    public class CollisionSystem(EntityManager entityManager)
        : ASystem(
            entityManager,
            [
                ComponentType.Collision,
                ComponentType.Transform,
                ComponentType.Graphics
            ]
        )
    {
        // Buffer storage for one-frame collisions check - gathers all collidable entities for future check in PerformSystemAction()
        private List<FloatRect> _collidableRects = [];
        private readonly FloatRect ZERO_RECT = new (0, 0, 0, 0);

        private void GatherCollidableRects() {
            Dictionary<ComponentType, Component> result = [];
            // receive a whole bunch of components for each entity
            var collidables = _entityManager.GetAllEntitiesWith([ComponentType.Collision, ComponentType.Graphics]);
            // filter only needed components
            foreach (var entity in collidables) {
                GraphicsComponent gc = (GraphicsComponent) entity.First(c => c.Type == ComponentType.Graphics);
                _collidableRects.Add(gc.Sprite.GetGlobalBounds());
            }
        }

        private FloatRect CheckCollision(FloatRect entity, FloatRect obstacle) {
            entity.Intersects(obstacle, out FloatRect intersection);
            return intersection;
        }

        public bool FitInScreenBounds(FloatRect bounds, TransformComponent entity)
        {
            // Check the left border
            if (bounds.Left < 0) {
                entity.SetX(0);
                return true;
            }
            // Check the right border
            if (bounds.Left + bounds.Width > AppConstants.CANVAS_WIDTH) {
                entity.SetX(AppConstants.CANVAS_WIDTH - bounds.Width);
                return true;
            }
            return false;
        }

        public override void Update()
        {
            // Reset the collidables list
            GatherCollidableRects();
            // Continue with the base method 
            base.Update();
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            // Skip if entity hasn't moved
            var transformComponent  = (TransformComponent)  entityComponents[ComponentType.Transform];
            if (transformComponent.HasMoved) return;
            
            var graphicsComponent   = (GraphicsComponent)   entityComponents[ComponentType.Graphics];
            var physicsComponent    = (PhysicsComponent)    entityComponents[ComponentType.Physics];

            FloatRect entityRect = graphicsComponent.Sprite.GetGlobalBounds();

            // - Iterate through borders
            if (FitInScreenBounds(entityRect, transformComponent)) {
                // Stop the entity
                physicsComponent.Stop();
            }

            // - Iterate through entities
            foreach (var collidable in _collidableRects) {
                var intersection = CheckCollision(entityRect, collidable);
                // IF collision occured
                if (intersection != ZERO_RECT) {
                    // Find direction of the intersection
                    float deltaX = intersection.Width < intersection.Height ? intersection.Left : 0;
                    float deltaY = intersection.Height < intersection.Width ? intersection.Top  : 0;

                    // Deduce from which side did collision occure
                    if (deltaX != 0)
                    {
                        // Offset depending on the sign of deltaX
                        var xOffset = deltaX > 0 ? -intersection.Width : intersection.Width;
                        transformComponent.Move(xOffset, 0);
                        // Stop the entity
                        physicsComponent.Stop();
                    }
                    else if (deltaY != 0)
                    {
                        // Offset depending on the sign of deltaY
                        var yOffset = deltaY > 0 ? -intersection.Height : intersection.Height;
                        transformComponent.Move(0, yOffset);
                        // Set entity as no longer falling
                        physicsComponent.Ground();
                    }
                }
            }
        }
    }
}