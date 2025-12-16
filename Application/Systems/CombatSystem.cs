using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class CombatSystem(EntityManager entityManager)
        : ASystem(
            entityManager,
            [
                ComponentType.Combat,
                ComponentType.Transform,
                // todo: TEMP [1] Graphics Component present as well
                ComponentType.Graphics
            ]
        )
    {
        private List<(CombatComponent, SFML.Graphics.FloatRect)> _warriors = [];

        public override void Update()
        {
            _warriors = [];
            bool playerAlive = false;
            var selectedEntities = _entityManager.GetAllComponentBundlesWith([ComponentType.Combat, ComponentType.Transform]);
            // Fills list with all entites that have CC and TC and are currently in the game
            foreach (var entity in selectedEntities) {
                var tc = (TransformComponent) entity.First(c => c.Type == ComponentType.Transform);
                SFML.Graphics.FloatRect hitbox = new(tc.X, tc.Y-tc.Height, tc.Width, tc.Height);
                var cc = (CombatComponent)entity.First(c => c.Type == ComponentType.Combat);
                var bundle = (cc, hitbox);

                // Check for whether it's player and if it is alive
                if (entity.FirstOrDefault(c => c.Type == ComponentType.Input) != null) {
                    if (!cc.IsDead) playerAlive = true;
                }

                if (!cc.IsDead) _warriors.Add(bundle);  // adds only entities that are alive 
            }

            // Check if game should continue
            if (!playerAlive) {
                // * Game Over
            }
            if (_warriors.Count == 1) {
                // * Only player remains – Victory
            }
            
            base.Update();
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            var combatComponent     = (CombatComponent)     entityComponents[ComponentType.Combat];
            var transformComponent  = (TransformComponent)  entityComponents[ComponentType.Transform];
            // todo: TEMP [1] Graphics Component present as well
            var graphicsComponent   = (GraphicsComponent)   entityComponents[ComponentType.Graphics];

            // Ensure the "dead" entities are not being handled
            if (combatComponent.IsDead) {
                // todo: Reset Texture or State (<- which's not Impl. yet)
                graphicsComponent.SetTexture("tombstone.png");
                // _entityManager.RemoveComponent(?ihavenoentity, ComponentType.Collision); // todo: removing collision would be nice
                return;
            }

            combatComponent.Update();

            if (combatComponent.IsDealingDamage) {
                // * Create imaginary FloatRect for attacked area.
                var aX = transformComponent.Direction == 1 ?
                        transformComponent.X + transformComponent.Width : 
                        transformComponent.X - combatComponent.AttackRange;
                SFML.Graphics.FloatRect attackArea = new(aX, transformComponent.Y - transformComponent.Height,
                                                        combatComponent.AttackRange, transformComponent.Height);

                // * Check if it collides with any entity that has CombatComponent
                var thisTop = transformComponent.Y - transformComponent.Height;
                foreach (var body in _warriors) {
                    // * Return if entity is this entity (to not check itself)
                    // instead of id-check I want to try somewhat simplier approach with checking rects
                    // ..as entities with combatCs usually have collisions as well.
                    if (body.Item2.Top == thisTop && body.Item2.Left == transformComponent.X) {
                        continue;
                    } 
                    
                    if (CollisionSystem.AreColliding(attackArea, body.Item2)) {
                        body.Item1.TakeDamage(combatComponent.Damage);
                        Console.WriteLine($"{body.Item1.Health}");
                        combatComponent.HasAttacked = true;
                    }
                }
            }

            // // Remove entity if its HP dropped to/below 0
            // if (combatComponent.IsDead) _entityManager.RemoveEntity();  // ? (._ .)
        }
    }
}