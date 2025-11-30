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
                ComponentType.Physics,
                ComponentType.Graphics
            ]
        )
    {
        // Buffer storage for one-frame collisions check - gathers all collidable entities for future check in PerformSystemAction()
        private Dictionary<Entity, FloatRect> _collidableRects = [];
        private Dictionary<Entity, (FloatRect, TransformComponent, PhysicsComponent?)> _movedEntities = [];
        private readonly FloatRect ZERO_RECT = new (0, 0, 0, 0);

        private void GatherCollidableRects() {
            _collidableRects.Clear();
            _movedEntities.Clear();
            // receive a whole bunch of components for each entity
            var collidables = _entityManager.GetAllEntitiesWith([ComponentType.Collision, ComponentType.Graphics]);
            // filter only needed components
            foreach (var entity in collidables) {
                // Add Entity to the check-list
                GraphicsComponent gc = (GraphicsComponent) entity.Value.First(c => c.Type == ComponentType.Graphics);
                FloatRect entityBounds = gc.Sprite.GetGlobalBounds();
                _collidableRects.Add(entity.Key, entityBounds);
                
                // * Note: not the most elegant solution, as all here, but will do perfectly fine at this scale >
                // Add moved entities to the separate list as well
                TransformComponent tc = (TransformComponent) entity.Value.First(c => c.Type == ComponentType.Transform);
                if (tc.HasMoved) {
                    PhysicsComponent? phc = (PhysicsComponent?) entity.Value.FirstOrDefault(c => c.Type == ComponentType.Physics);
                    _movedEntities.Add(entity.Key, (entityBounds, tc, phc));
                }
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
            // // Continue with the base method 
            // base.Update();
            foreach (var movedEntity in _movedEntities) {
                PerformCustomSystemAction(movedEntity);
            }
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {}
        protected void PerformCustomSystemAction(KeyValuePair<Entity, (FloatRect, TransformComponent, PhysicsComponent?)> entity) {
            FloatRect           entityRect          = entity.Value.Item1;
            TransformComponent  transformComponent  = entity.Value.Item2;
            PhysicsComponent?   physicsComponent    = entity.Value.Item3;

            // - Iterate through borders
            if (FitInScreenBounds(entityRect, transformComponent)) {
                // Stop the entity
                physicsComponent?.Stop();
            }

            // - Iterate through entities
            foreach (var collidable in _collidableRects) {
                if (collidable.Key == entity.Key) continue; // to skip checking collision with itself
                var intersection = CheckCollision(entityRect, collidable.Value);
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
                        // ? var xOffset = -deltaX;
                        transformComponent.ChangePostition(xOffset, 0);
                        // Stop the entity
                        physicsComponent?.Stop();
                    }
                    if (deltaY != 0)
                    {
                        // Offset depending on the sign of deltaY
                        var yOffset = deltaY > 0 ? -intersection.Height : intersection.Height;
                        transformComponent.ChangePostition(0, yOffset);
                        // Set entity as no longer falling
                        physicsComponent?.Ground();
                    }
                }
            }
        }
    }
}