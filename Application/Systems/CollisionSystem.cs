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
        private Dictionary<Entity, (FloatRect, TransformComponent, PhysicsComponent?, bool)> _movedEntities = [];
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
                TransformComponent tc = (TransformComponent) entity.Value.First(c => c.Type == ComponentType.Transform);
                // FloatRect entityBounds = gc.Sprite.GetGlobalBounds();
                var spriteBoundsRect = gc.Sprite.GetLocalBounds();
                FloatRect entityBounds = new(tc.X, tc.Y, spriteBoundsRect.Width, spriteBoundsRect.Height);
                _collidableRects.Add(entity.Key, entityBounds);
                
                // * Note: not the most elegant solution, as all here, but will do perfectly fine at this scale >
                // Add moved entities to the separate list as well
                if (tc.HasMoved) {
                    PhysicsComponent? phc = (PhysicsComponent?) entity.Value.FirstOrDefault(c => c.Type == ComponentType.Physics);
                    _movedEntities.Add(entity.Key, (entityBounds, tc, phc, false));
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
                return false;
            }
            // Check the right border
            if (bounds.Left + bounds.Width > AppConstants.CANVAS_WIDTH) {
                entity.SetX(AppConstants.CANVAS_WIDTH - bounds.Width);
                return false;
            }
            return true;
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
        protected void PerformCustomSystemAction(KeyValuePair<Entity, (FloatRect, TransformComponent, PhysicsComponent?, bool)> entity) {
            FloatRect           entityRect          = entity.Value.Item1;
            TransformComponent  transformComponent  = entity.Value.Item2;
            PhysicsComponent?   physicsComponent    = entity.Value.Item3;
            bool                hasCollidedOnBottom = entity.Value.Item4;

            // ! Debug log
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n- - - COLLISION CHECK ROUND for {entity.Key.Id} - - -");
            Console.ResetColor();
            Console.WriteLine($"Entity's coords\t-\tX: {entity.Value.Item2.X}\tY: {entity.Value.Item2.Y}");

            // - Iterate through entities
            foreach (var collidable in _collidableRects) {
                // Skip the check if collidable is this entity (collision with itself)
                if (collidable.Key.Id == entity.Key.Id) continue;

                // Find intersection area
                FloatRect newEntityRect = new(transformComponent.X, transformComponent.Y, entityRect.Width, entityRect.Height);
                var intersection = CheckCollision(newEntityRect, collidable.Value);
                
                // ! Debug log
                if (collidable.Key.Id == 9) {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"\nFloor overlap with Entity >");
                    Console.WriteLine($"intersection.Width: {intersection.Width}\nintersection.Height: {intersection.Height}");
                    Console.WriteLine($"intersection.Top: {intersection.Top}\nintersection.Left: {intersection.Left}");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nFloor FloatRect >");
                    Console.WriteLine($"Width: {collidable.Value.Width}\nHeight: {collidable.Value.Height}");
                    Console.WriteLine($"Top: {collidable.Value.Top}\nLeft: {collidable.Value.Left}");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"\nEntity FloatRect >");
                    Console.WriteLine($"Width: {entityRect.Width}\nHeight: {entityRect.Height}");
                    Console.WriteLine($"Top: {entityRect.Top}\nLeft: {entityRect.Left}");
                    Console.ResetColor();
                }

                // * If collision occured
                if (intersection != ZERO_RECT) {
                    
                    // ! Debug log
                    Console.WriteLine($"\n- Collision occured with entity {collidable.Key.Id} >");
                    Console.WriteLine($"intersection.Width: {intersection.Width}\nintersection.Height: {intersection.Height}");
                    Console.WriteLine($"intersection.Top: {intersection.Top}\nintersection.Left: {intersection.Left}");

                    // If X offset is greater than on Y axis - rise (or lower) the entity
                    // if (intersection.Width > intersection.Height) {
                        var intersectionY = intersection.Top /*+ intersection.Height // as it's already bottom in SFML for some reason*/;
                        float yOffset;
                        if (intersectionY >= transformComponent.Y) {
                            yOffset = -intersection.Height;
                            // if entity collided with smth on the bottom
                            physicsComponent?.Ground();
                            hasCollidedOnBottom = true;

                            // ! Debug log
                            Console.WriteLine($"(entity {entity.Key.Id} touched floor - no longer falling)");
                        }
                        else {
                            yOffset = intersection.Height;
                        }
                        transformComponent.ChangePostition(0, yOffset);
                    // }


                    // else {
                    //     var xOffset = intersection.Left > transformComponent.X ? -intersection.Width : intersection.Width;
                    //     transformComponent.ChangePostition(xOffset, 0);
                    //     // Stop the entity
                    //     physicsComponent?.Stop();
                    // }

                    // ! Debug log
                    Console.WriteLine($"After fix\t-\tX: {entity.Value.Item2.X}\tY: {entity.Value.Item2.Y}");
                }
            }

            // ! Debug log
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n- - - END - - -");
            Console.ResetColor();

            // If the entity has never collided on the bottom it is falling
            if (!hasCollidedOnBottom && physicsComponent is not null) {physicsComponent.IsFalling = true;
                // ! Debug log
                Console.WriteLine($"(entity {entity.Key.Id} is falling now)");}

            // - Iterate through borders
            if (!FitInScreenBounds(entityRect, transformComponent)) {
                // Stop the entity
                physicsComponent?.Stop();
            }
        }

        public static bool AreColliding(FloatRect a, FloatRect b) {
            return a.Intersects(b);
        }
    }
}