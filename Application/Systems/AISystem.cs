using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class AISystem : ASystem
    {
        private readonly (TransformComponent, CombatComponent) _player;
        private readonly Dictionary<Entity, List<Component>> _enemies;

        public AISystem(EntityManager entityManager) : base(
            entityManager,
            [
                ComponentType.AI,
                ComponentType.Combat,
                ComponentType.Transform,
                ComponentType.Physics,
            ])
        {
            var player = _entityManager.GetAllComponentBundlesWith([ComponentType.Input])[0];
            var playerTC = (TransformComponent) player.First(c => c.Type == ComponentType.Transform);
            var playerCC = (CombatComponent)    player.First(c => c.Type == ComponentType.Combat);
            _player = (playerTC, playerCC);

            _enemies = _entityManager.GetAllEntitiesWith([ComponentType.AI]);
        }

        private void MoveEnemy(PhysicsComponent phc) {
            // smth*
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            var aiComponent = (AIComponent) entityComponents[ComponentType.AI];
            var combatComponent = (CombatComponent) entityComponents[ComponentType.Combat];
            var transformComponent = (TransformComponent) entityComponents[ComponentType.Transform];
            var physicsComponent = (PhysicsComponent) entityComponents[ComponentType.Physics];

            float distToPlayer = transformComponent.X - _player.Item1.X;
    
            // State transitions
            if (distToPlayer < combatComponent.AttackRange) {
                aiComponent.CurrentState = AIState.Attack;
            } else if (distToPlayer < aiComponent.AggroRange /* * aiComponent.Aggression*/) {
                aiComponent.CurrentState = AIState.Chase;
            } else {
                aiComponent.CurrentState = AIState.Patrol;
            }
            
            // State behaviors
            switch (aiComponent.CurrentState) {
                case AIState.Patrol:
                    PatrolBehavior(transform, physics, aiComponent);
                    break;
                case AIState.Chase:
                    ChaseBehavior(transform, physics, playerTransform, aiComponent);
                    break;
                case AIState.Attack:
                    AttackBehavior(physics, combat, transform, playerTransform);
                    break;
            }
        }
    }
}