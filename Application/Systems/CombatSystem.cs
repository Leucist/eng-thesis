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
        // ! DEBUG TEMP
        private Entity _playerId;

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
                    _playerId = entity.Key;
                }

                // Only if entity is alive, add it
                if (!cc.IsDead) _warriors[entity.Key] = bundle; 
            }

            // Check if game should continue
            if (!playerAlive) {
                // * Game Over
                _isWorldAlive.Value = false;
                GraphicsUtils.WindowManager.GameOver("GAME OVER!");
            }
            else if (_warriors.Count == 1) {
                // * Only player remains – Victory
                _isWorldAlive.Value = false;
                GraphicsUtils.WindowManager.GameOver("VICTORY!");
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

            // If entity is able to hit anyone in this frame (is dealing damage)
            if (combatComponent.IsDealingDamage) {
                // ! DEBUG LOG
                Console.WriteLine($"> Entity {entity.Key.Id} at X: {transformComponent.X} is dealing {combatComponent.Damage} damage.");
                
                // * Create imaginary FloatRect for attacked area.
                var attackRectX = transformComponent.Direction == 1 ?
                        transformComponent.X + transformComponent.Width : 
                        transformComponent.X - combatComponent.AttackRange;
                SFML.Graphics.FloatRect attackArea = new(attackRectX, transformComponent.Y - transformComponent.Height,
                                                        combatComponent.AttackRange, transformComponent.Height);

                // Iterate through all the entities with Combat Component
                foreach (var victim in _warriors) {
                    // * Skip iteration if entity is this entity (to not check itself)
                    if (victim.Key == entity.Key) continue;
                    
                    // Init the hitbox of the "victim"
                    var victimTC = victim.Value.Item2;
                    SFML.Graphics.FloatRect victimHitbox = new(victimTC.X, victimTC.Y-victimTC.Height, victimTC.Width, victimTC.Height);

                    // * Check if attacked area collides with any entity that has CombatComponent
                    if (CollisionSystem.AreColliding(attackArea, victimHitbox)) {
                        victim.Value.Item1.TakeDamage(combatComponent.Damage);
                        combatComponent.HasAttacked = true;

                        // ! DEBUG LOG
                        Console.WriteLine($"\t· Entity {victim.Key.Id} was just hit.");
                        Console.WriteLine($"\t· Entity's HP: {victim.Value.Item1.Health}.");

                        // * Ensure the "dead" entities are not being handled
                        if (victim.Value.Item1.IsDead) {
                            // todo: Reset Texture or State (<- which's not Impl. yet)
                            victim.Value.Item3?.SetTexture("tombstone.png");
                            _entityManager.RemoveComponent(victim.Key, ComponentType.Collision);    // remove collision
                            return;
                        }
                    }
                }
            }
            else if (entity.Key != _playerId) {
                // ! DEBUG LOG
                Console.WriteLine($"-- Somehow {entity.Key.Id} is not dealing damage...");
                Console.WriteLine($"My stats:");
                Console.WriteLine($"-- IsDealingDamage: {combatComponent.IsDealingDamage}");
                Console.WriteLine($"-- HasAttacked: {combatComponent.HasAttacked}");
                Console.WriteLine($"-- _attackCounter: {combatComponent._attackCounter}");
                Console.WriteLine($"-- AttackDuration: {combatComponent.AttackDuration}\n");
            }
        }

        public void LinkWorldLife(ref Worlds.BoolWrapper isAlive) {
            _isWorldAlive = isAlive;
        }
    }
}