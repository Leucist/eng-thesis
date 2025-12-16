namespace Application.Components
{
    public class CombatComponent : Component
    {
        private const float HP_REGENED_PER_FRAME = 0.5f;

        public float    Damage          { get; private set; }
        public float    AttackRange     { get; private set; }
        public int      AttackCooldown  { get; private set; }     // How long to wait before next attack 
        public int      AttackDuration  { get; private set; }     // How long the attack can cause damage
        private int _attackCounter;
        // * ^ e,g, Player has attack animation for 4 frames, but only first 3 deal damage

        public float    Health          { get; private set; }
        public float    MaxHealth       { get; private set; }
        public int      HPRegenCooldown { get; private set; }
        private int _regenCounter;

        public bool HasAttacked = false;
        
        public bool IsDealingDamage => !HasAttacked && (_attackCounter <= AttackDuration);    // Whether the entity is able to deal damage now
        public bool CanAttack       => _attackCounter <= AttackCooldown;    // Whether the entity can launch a new attack now
        public bool IsDead => Health <= 0;
        
        public CombatComponent(float damage, float attackRange, int attackCooldown, int attackDuration, 
                            float health, float maxHealth, int hpRegenCooldown) 
            : base(ComponentType.Combat)
        {
            Damage = damage;
            AttackRange = attackRange;
            AttackCooldown = attackCooldown;
            AttackDuration = Math.Min(attackDuration, AttackCooldown);
            Health = health;
            MaxHealth = maxHealth;
            HPRegenCooldown = hpRegenCooldown;

            _attackCounter  = AttackCooldown;
            _regenCounter   = 0;
        }

        public void Attack() {
            _attackCounter = 0;
        }

        public void TakeDamage(float damage) {
            Health -= damage;
        }

        public void Update() {
            // * Method is about to be called each frame:
            // Health Points    - are being regenerated to their max value each 1 in HPRegenCooldown frames by HP_REGENED_PER_FRAME.
            // _attackCounter   - rises (+1 per frame) and stays at AttackCooldown untill starts an attack - then counter is set to 0.
            

            if (_attackCounter < AttackCooldown) {
                _attackCounter++;
            }
            else HasAttacked = false;
            
            if (_regenCounter <= 0) {
                var increasedHP = Health + HP_REGENED_PER_FRAME;
                // increase Health Points so they won't breach the max limit
                Health = increasedHP > MaxHealth ? MaxHealth : increasedHP;
                _regenCounter = HPRegenCooldown;
            }
            else _regenCounter--;
        }
    }
}