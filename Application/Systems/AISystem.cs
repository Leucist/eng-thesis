using Application.AppMath;
using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public class AISystem : ASystem
    {
        private readonly (TransformComponent, CombatComponent) _player;
        // private readonly Dictionary<Entity, List<Component>> _enemies;

        // private const float ATTACK_DISTANCE_MULTIPLIER = 2.5f;
        private const int ATTACK_DISTANCE_OFFSET = 32;

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

            // _enemies = _entityManager.GetAllEntitiesWith([ComponentType.AI]);
        }

        protected override void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents) {
            var aiComponent = (AIComponent) entityComponents[ComponentType.AI];
            var combatComponent = (CombatComponent) entityComponents[ComponentType.Combat];
            var transformComponent = (TransformComponent) entityComponents[ComponentType.Transform];
            var physicsComponent = (PhysicsComponent) entityComponents[ComponentType.Physics];

            // Skip dead enemies
            if (combatComponent.IsDead) return;

            float XdistToPlayer = (_player.Item1.X + _player.Item1.Width/2) - (transformComponent.X + transformComponent.Width/2);
            float YdistToPlayer = (_player.Item1.Y + _player.Item1.Height/2) - (transformComponent.Y + transformComponent.Height/2);
            // float YdistToPlayer = _player.Item1.Y - transformComponent.Y;
            float distToPlayer = MathF.Sqrt((XdistToPlayer * XdistToPlayer) + (YdistToPlayer * YdistToPlayer));

            // Increment time in current state
            aiComponent.TimeInState++;
            
            // Random reaction delay - don't make decisions every frame
            if (--aiComponent.NextDecisionTime > 0 && aiComponent.CurrentState != AIState.AttackWindup)
            {
                // Still execute current state behavior, just don't make new decisions
                ExecuteStateBehavior(aiComponent, combatComponent, physicsComponent, XdistToPlayer, transformComponent);
                return;
            }
            
            // Time to make a new decision
            aiComponent.NextDecisionTime = aiComponent.DecisionDelay;
            
            // Death run: Check if health is low
            float healthPercent = combatComponent.Health / combatComponent.MaxHealth;
            bool shouldFlee = healthPercent <= aiComponent.FleeHealthThreshold;
            
            // State transition logic
            if (shouldFlee && aiComponent.CurrentState != AIState.Flee)
            {
                ChangeState(aiComponent, AIState.Flee);
            }
            else if (aiComponent.CurrentState == AIState.AttackWindup)
            {
                // Handle attack windup timer
                aiComponent.AttackWindupTimer++;
                if (aiComponent.AttackWindupTimer >= aiComponent.AttackWindupDuration)
                {
                    ChangeState(aiComponent, AIState.Attack);
                    aiComponent.AttackWindupTimer = 0;
                }
            }
            else if (aiComponent.CurrentState == AIState.Chase)
            {
                // Give up chase if too long or player too far
                if (aiComponent.TimeInState > aiComponent.ChaseGiveUpTime && 
                    distToPlayer > aiComponent.AggroRange + aiComponent.Aggression 
                    || MathF.Abs(YdistToPlayer) > _player.Item1.Height /* if player is so high that it's out of reach */)
                {
                    transformComponent.SetDirection(0);
                    ChangeState(aiComponent, AIState.Patrol);
                }

                else if (distToPlayer < combatComponent.AttackRange + ATTACK_DISTANCE_OFFSET)
                {
                    ChangeState(aiComponent, AIState.AttackWindup);  // Start windup instead of immediate attack
                }
            }
            else if (!shouldFlee)
            {
                // Normal state transitions
                if (distToPlayer < combatComponent.AttackRange + ATTACK_DISTANCE_OFFSET)
                {
                    ChangeState(aiComponent, AIState.AttackWindup);
                }
                else if (distToPlayer < aiComponent.AggroRange 
                        && MathF.Abs(YdistToPlayer) < _player.Item1.Height /* if player is so high that it's out of reach */)
                {
                    ChangeState(aiComponent, AIState.Chase);
                }
                else if (aiComponent.CurrentState != AIState.Patrol)
                {
                    ChangeState(aiComponent, AIState.Patrol);
                }
            }
            
            // Execute behavior for current state
            ExecuteStateBehavior(aiComponent, combatComponent, physicsComponent, XdistToPlayer, transformComponent);
        }

        private void ChangeState(AIComponent ai, AIState newState)
        {
            if (ai.CurrentState != newState)
            {
                ai.CurrentState = newState;
                ai.TimeInState = 0;
                ai.PatrolReconsiderDirectionTimer = 0;
                
                // Reset attack windup timer when entering windup state
                if (newState == AIState.AttackWindup)
                {
                    ai.AttackWindupTimer = 0;
                }
            }
        }

        private void ExecuteStateBehavior(AIComponent ai, CombatComponent combat, PhysicsComponent physics, float XdistToPlayer, TransformComponent transform)
        {
            switch (ai.CurrentState)
            {
                case AIState.Patrol:
                    PatrolBehavior(transform, physics, ai);
                    break;
                    
                case AIState.Chase:
                    ChaseBehavior(XdistToPlayer, physics, ai);
                    break;
                    
                case AIState.AttackWindup:
                    // Pause/telegraph before attack - no movement
                    WindupBehavior(physics);
                    break;
                    
                case AIState.Attack:
                    AttackBehavior(physics, combat);
                    // After attack, go back to chase or patrol based on distance
                    // ChangeState(ai, distToPlayer < ai.AggroRange ? AIState.Chase : AIState.Patrol);  //? kinda redundant~
                    break;
                    
                case AIState.Flee:
                    FleeBehavior(XdistToPlayer, physics, ai);
                    break;
            }
        }

        private void PatrolBehavior(TransformComponent transform, PhysicsComponent physics, AIComponent ai)
        {
            if (ai.PatrolReconsiderDirectionTimer >= ai.PatrolReconsiderDirectionTime) {
                // Simple left-right patrol
                // int direction = MathF.Sin(ai.TimeInState) > 0 ? 1 : -1;
                var random = new Random();
                transform.SetDirection(random.Next(2) > 0 ? 1 : -1);
                ai.PatrolReconsiderDirectionTimer = 0;
            }
            else {
                ai.PatrolReconsiderDirectionTimer++;
            }
            AddMovementForce(physics, ai.PatrolSpeed, transform.Direction);
        }

        private void ChaseBehavior(float XdistToPlayer, PhysicsComponent physics, AIComponent ai)
        {
            AddMovementForce(physics, ai.ChaseSpeed, Math.Sign(XdistToPlayer));
        }

        private void WindupBehavior(PhysicsComponent physics)
        {
            // Stop movement during windup (telegraph)
            physics.Stop();
        }

        private void AttackBehavior(PhysicsComponent physics, CombatComponent combat)
        {
            if (combat.CanAttack)
            {
                // Stop moving
                physics.Stop();
                // Execute attack
                combat.Attack();
            }
        }

        private void FleeBehavior(float distToPlayer, PhysicsComponent physics, AIComponent ai)
        {
            // Run away from player
            AddMovementForce(physics, ai.FleeSpeed, -Math.Sign(distToPlayer));
        }

        private void AddMovementForce(PhysicsComponent physics, int magnitude, int direction)
        {
            var angle = direction > 0 ? MathConstants.RadiansRightDirection : MathConstants.RadiansLeftDirection;
            physics.AddAppliedForce(new(magnitude, angle));
        }
    }
}