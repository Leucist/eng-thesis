namespace Application.Components
{
    public enum AIState {
        Idle,
        Patrol,
        Chase,
        Attack,
        AttackWindup,
        Flee    // <- option for low-health cowardly skeletons :P
    }

    public class AIComponent : Component
    {
        public AIState CurrentState = AIState.Patrol;
        
        // Ranges and speeds
        public float AggroRange = 150f;
        public float AttackRange = 40f;
        public float Aggression = 1f;               // may later use random ~0.5-1.5 range for personality
        public int PatrolSpeed = 15;
        public int ChaseSpeed = 20;
        public int FleeSpeed = 22;
        
        // Timers and delays
        public float NextDecisionTime = 0f;
        public float DecisionDelay;                 // randomized delay for any decision
        public int TimeInState = 0;
        public int AttackWindupDuration = 1;
        public int AttackWindupTimer    = 0;
        public int ChaseGiveUpTime = 35;         // enemy gives up after this many frames
        public int PatrolReconsiderDirectionTime = 24;
        public int PatrolReconsiderDirectionTimer;
        
        // Health-based behavior
        public float FleeHealthThreshold = 0.15f;   // 15% HP
        
        // public float PatrolTargetX;
        
        // Constructor to randomize personality
        public AIComponent() : base(ComponentType.AI)
        {
            PatrolReconsiderDirectionTimer = PatrolReconsiderDirectionTime;
            Aggression = 0.7f + (float)(Random.Shared.NextDouble() * 0.6);      // 0.7-1.3
            DecisionDelay = 0.1f + (float)(Random.Shared.NextDouble() * 0.2);   // 0.1-0.3s
        }
    }
}