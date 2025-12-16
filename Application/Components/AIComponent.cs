namespace Application.Components
{
    public enum AIState {
        Idle,
        Patrol,
        Chase,
        Attack,
        Flee    // <- optional for low-health cowardly skeletons :P
    }

    public class AIComponent : Component
    {
        public AIState CurrentState = AIState.Patrol;
        
        // Ranges and speeds
        public float AggroRange = 150f;
        public float AttackRange = 40f;
        public float Aggression = 1f;               // may later use random ~0.5-1.5 range for personality
        public float PatrolSpeed = 15f;
        public float ChaseSpeed = 25f;
        public float FleeSpeed = 30f;
        
        // Timers and delays
        public float NextDecisionTime = 0f;
        public float DecisionDelay;                 // randomized delay for any decision
        public float TimeInState = 0f;
        public float AttackWindupDuration = 0.2f;
        public float AttackWindupTimer = 0f;
        public float ChaseGiveUpTime = 3.5f;        // enemy gives up after this many frames
        
        // Health-based behavior
        public float FleeHealthThreshold = 0.15f;   // 15% HP
        
        public float PatrolTargetX;
        
        // Constructor to randomize personality
        public AIComponent() : base(ComponentType.AI)
        {
            Aggression = 0.7f + (float)(Random.Shared.NextDouble() * 0.6);      // 0.7-1.3
            DecisionDelay = 0.1f + (float)(Random.Shared.NextDouble() * 0.2);   // 0.1-0.3s
        }
    }
}