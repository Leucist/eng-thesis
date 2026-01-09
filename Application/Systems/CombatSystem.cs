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
        private AI.AIDistributionManager? _aiManager;

        private const int ENEMIES_AMOUNT = 4;
        private readonly (int X, int Y)[] SPAWN_POINTS = [
            (256, 288), // close
            (416, 288), // mid
            // (544, 256), // far
            (512, 256), // far
        ];

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

                // Only if entity is alive, add it
                if (!cc.IsDead) _warriors[entity.Key] = bundle; 
            }

            // Check if game should continue
            if (!playerAlive) {
                // * [ GAME OVER ]
                _aiManager!.RecordAIWin();
                _isWorldAlive.Value = false;
                GraphicsUtils.WindowManager.GameOver("GAME OVER!");
            }
            else if (_warriors.Count == 1) {
                // * [ VICTORY ] - Only player remains
                _aiManager!.RecordPlayerWin();
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
        }

        public void LinkWorldLife(ref Worlds.BoolWrapper isAlive) {
            _isWorldAlive = isAlive;
        }
        public void LinkAIManager(AI.AIDistributionManager aiManager) {
            _aiManager = aiManager;
            SpawnEnemies(); // <- as it's called when Level is created
        }

        private (Entity Entity, List<Component> Components) CreateEnemy() {
            // TODO: Extract in some EntityFactory class, maybe~ // Left here for now
            // Create entity
            var enemy = _entityManager.CreateEntity();

            // Load and deserialize enemy template
            string json = File.ReadAllText(Pathfinder.GetCharacterPrefabPath("menu_ninja_enemy"));
            System.Text.Json.Nodes.JsonNode root = System.Text.Json.Nodes.JsonNode.Parse(json)!;
            System.Text.Json.Nodes.JsonArray componentsNode = (System.Text.Json.Nodes.JsonArray) root["Components"]!;
            string componentsJson = componentsNode.ToJsonString();
            List<Component> components = System.Text.Json.JsonSerializer.Deserialize<List<Component>>(componentsJson)!;

            // Add components
            _entityManager.AddComponents(enemy, components);

            return (enemy, components);
        }

        private void SpawnEnemyAt((int X, int Y) spawnpoint, float xOffset=0f) {
            var newEnemy = CreateEnemy();

            TransformComponent tc = (TransformComponent) newEnemy.Components.First(c => c.Type == ComponentType.Transform);
            tc.SetX(spawnpoint.X + xOffset);
            tc.SetY(spawnpoint.Y);
        }

        private void SpawnEnemies() {
            float[] distribution = _aiManager!.Distribution;
            //*^ [0.4, 0.3, 0.3] for example

            // Fill weightedPoints
            var weightedPoints = new (int Count, int SpawnPoint)[ENEMIES_AMOUNT];
            for (int i = 0; i < SPAWN_POINTS.Length; i++) {
                weightedPoints[i] = ((int) Math.Round(distribution[i] * ENEMIES_AMOUNT, MidpointRounding.AwayFromZero), i);
            }

            // Sort spawn point by their enemy distribution descending
            Array.Sort(weightedPoints, (a, b) => b.Count.CompareTo(a.Count));

            // Spawn enemies
            int enemiesSpawned = 0;
            foreach (var (Count, SpawnPoint) in weightedPoints) {
                for (int i = 0; i < Count && enemiesSpawned < ENEMIES_AMOUNT; i++) {
                    // ? If needed, may add small offset like SP.X += 0.01f * i here for enemy crowds~
                    SpawnEnemyAt(SPAWN_POINTS[SpawnPoint], 0.1f * i);
                    enemiesSpawned++;
                }
            }
        }
    }
}