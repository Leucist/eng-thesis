using Application.Entities;
using Application.Components;
using Application.Systems;
using Application.GraphicsUtils;

using System.Text.Json;

namespace Application.Worlds
{
    public class BoolWrapper
    {
        public bool Value { get; set; }

        // Implicit conversion operator from BoolWrapper to bool
        public static implicit operator bool(BoolWrapper wrapper)
        {
            return wrapper.Value;
        }

        // Implicit conversion operator from bool to BoolWrapper
        public static implicit operator BoolWrapper(bool value)
        {
            return new BoolWrapper { Value = value };
        }
    }

    public class World
    {
        private readonly EntityManager _entityManager;
        private readonly List<ASystem> _systems;
        // private readonly Entity _player = player;
        private BoolWrapper _isAlive;
        private AI.AIDistributionManager? _aiManager;

        public void LinkAIManager(AI.AIDistributionManager aiManager) {
            _aiManager = aiManager;
        }

        public bool IsAlive  => _isAlive.Value;
        // public Entity Player => _player;

        public World(List<List<Component>> entities, List<string> systemTypes) {
            _systems = [];
            _entityManager = new EntityManager();

            // Fill Entities
            foreach (var componentBundle in entities) {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponents(entity, componentBundle);
            }

            // Initialize Systems
            foreach (var typeName in systemTypes) {
                // Find the matching class (type) or throw Exception
                Type type = Type.GetType(typeName) ?? throw new Exception($"System type {typeName} not found");
                // Create instance of the given type
                _systems.Add((ASystem) Activator.CreateInstance(type, _entityManager)!);
            }

            // todo: Not redundant if using level chain? Not yet, however~
            _isAlive = true;

            // todo: Temp? linking isAlive to combat.
            var cs = (CombatSystem?) _systems.FirstOrDefault(s => s.GetType() == typeof(CombatSystem));
            cs?.LinkWorldLife(ref _isAlive);
            cs?.LinkAIManager(_aiManager);
        }

        // * Separate method for Systems Init ?
        // - Check if Command System, add ref isAlive via .LinkWorldLife(bool) mb or so~
        // - * and there were other checks... hm.

        public void Update() {
            // Dispatch pending window events
            WindowManager.DispatchEvents();
            // Update systems
            foreach (var system in _systems) {
                system.Update();
            }
        }

        public WorldDTO ConvertToDTO() {
            var dto = new WorldDTO
            {
                Entities = _entityManager.GetAllEntities(),
                SystemTypes = _systems.Select(s => s.GetType().AssemblyQualifiedName!).ToList()
            };

            // todo: Background and size in tiles remain unused

            return dto;
        }

        /// <summary>
        /// Serializes World -> WorldDTO -> JSON file under Application/Source/Saves/{name}.json.
        /// Overwrites existing file.
        /// </summary>
        public void SaveWorldToFile(string name)
        {
            WorldDTO dto = ConvertToDTO();

            string path = Pathfinder.GetWorldSavePath(name);

            // todo: may be moved to a separate field
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true, // needed as the DTO uses public fields instead of properties
                PropertyNameCaseInsensitive = true
            };
            string json = JsonSerializer.Serialize(dto, options);
            
            File.WriteAllText(path, json);
        }
    }
}