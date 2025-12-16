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
        private Dictionary<Entity, (CombatComponent, TransformComponent, GraphicsComponent?)> _warriors = [];
        private Worlds.BoolWrapper _isWorldAlive = true;

        public override void Update()
        {
            _warriors = [];
            bool playerAlive = false;
            var selectedEntities = _entityManager.GetAllEntitiesWith([ComponentType.Combat, ComponentType.Transform]);
            // Fills list with all entites that have CC and TC and are currently in the game
            foreach (var entity in selectedEntities) {
                var tc = (TransformComponent)   entity.Value.First(c => c.Type == ComponentType.Transform);
                var cc = (CombatComponent)      entity.Value.First(c => c.Type == ComponentType.Combat);
                var gc = (GraphicsComponent?)   entity.Value.FirstOrDefault(c => c.Type == ComponentType.Graphics);
                var bundle = (cc, tc, gc);

                // Check for whether it's player and if it is alive
                if (entity.Value.FirstOrDefault(c => c.Type == ComponentType.Input) != null) {
                    if (!cc.IsDead) playerAlive = true;
                }

                if (!cc.IsDead) _warriors[entity.Key] = bundle;  // adds only entities that are alive 
            }

            // Check if game should continue
            if (!playerAlive) {
                // * Game Over
                _isWorldAlive.Value = false;
            }
            if (_warriors.Count == 1) {
                // * Only player remains – Victory
                _isWorldAlive.Value = false;
            }
            
            // base.Update();
            foreach (var entity in _warriors) {
                PerformCustomSystemAction(entity);
            }
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            throw new NotImplementedException("How did you call this anyway?");
        }
        protected void PerformCustomSystemAction(KeyValuePair<Entity, 
            (CombatComponent, TransformComponent, GraphicsComponent?)> entity)
        {
            var combatComponent     = entity.Value.Item1;
            var transformComponent  = entity.Value.Item2;
            // // todo: TEMP [1] Graphics Component present as well
            // var graphicsComponent   = entity.Value.Item3;

            combatComponent.Update();

            if (combatComponent.IsDealingDamage) {
                // * Create imaginary FloatRect for attacked area.
                var attackRectX = transformComponent.Direction == 1 ?
                        transformComponent.X + transformComponent.Width : 
                        transformComponent.X - combatComponent.AttackRange;
                SFML.Graphics.FloatRect attackArea = new(attackRectX, transformComponent.Y - transformComponent.Height,
                                                        combatComponent.AttackRange, transformComponent.Height);

                // * Check if it collides with any entity that has CombatComponent
                var thisTop = transformComponent.Y - transformComponent.Height;
                foreach (var victim in _warriors) {
                    // * Skip iteration if entity is this entity (to not check itself)
                    if (victim.Key == entity.Key) continue;
                    
                    // Init the hitbox of the "victim"
                    var victimTC = victim.Value.Item2;
                    SFML.Graphics.FloatRect victimHitbox = new(victimTC.X, victimTC.Y-victimTC.Height, victimTC.Width, victimTC.Height);
                    if (CollisionSystem.AreColliding(attackArea, victimHitbox)) {
                        victim.Value.Item1.TakeDamage(combatComponent.Damage);
                        combatComponent.HasAttacked = true;

                        // Ensure the "dead" entities are not being handled
                        if (victim.Value.Item1.IsDead) {
                            // todo: Reset Texture or State (<- which's not Impl. yet)
                            victim.Value.Item3?.SetTexture("tombstone.png");
                            _entityManager.RemoveComponent(victim.Key, ComponentType.Collision);    // remove collision
                            return;
                        }
                    }
                }
            }
        }

        public void LinkWorldLife(ref Worlds.BoolWrapper isAlive) {
            _isWorldAlive = isAlive;
        }
    }
}