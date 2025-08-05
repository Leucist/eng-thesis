using Application.Entities;
using Application.Systems;

namespace Application
{
    public class World
    {
        private List<ISystem> _systems;
        private bool _isAlive;
        private Entity _player;

        public bool IsAlive  => _isAlive;
        public Entity Player => _player;

        public World(Entity player) {
            _systems = [];
            _isAlive = true;
            _player = player;
        }

        public void Update() {
            foreach (var system in _systems) {
                system.Update();
            }
        }
    }
}