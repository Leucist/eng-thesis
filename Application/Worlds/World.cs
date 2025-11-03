using Application.Entities;
using Application.Components;
using Application.Systems;

namespace Application.Worlds
{
    public class World
    {
        private readonly EntityManager _entityManager;
        private readonly List<ASystem> _systems;
        // private readonly Entity _player = player;
        private bool _isAlive;

        public bool IsAlive  => _isAlive;
        // public Entity Player => _player;

        public World(List<List<Component>> entities, List<ASystem> systems) {
            _systems = systems;
            _entityManager = new EntityManager();

            // Fill Entities
            foreach (var componentBundle in entities) {
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponents(entity, componentBundle);
            }

            _isAlive = true;
        }

        // public World(List<Component> playerComponents) {
        //     // - Initialise attributes
        //     _entityManager = new EntityManager();
        //     _isAlive = true;
        //     _systems = [];
        //     // - Initialise the player entity
        //     CreatePlayer(playerComponents);

        //     // - Initialise Systems
        //     foreach (ASystem system in ) {}  // * LOADING FROM JSON-?
        //     // * Maybe not pass the player comp-s then, and just world init files – the MAP and Saves, any prefabs,
        //     // * ...or pass only additional components for the player? -> already has InputComponent and etc., but others passed
        // }

        private void CreatePlayer(List<Component> playerComponents) {
            Entity player = _entityManager.CreateEntity();
            foreach (var component in playerComponents) {
                _entityManager.AddComponent(player, component);
            }
        }

        public void Update() {
            foreach (var system in _systems) {
                system.Update();
            }
        }

        public WorldDTO ConvertToDTO() {
            var dto = new WorldDTO
            {
                Entities = _entityManager.GetAllEntities(),
                Systems = _systems
            };

            // todo: Background and size in tiles remain unused

            return dto;
        }
    }
}