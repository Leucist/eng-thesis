using Application.Entities;
using Application.Systems;

namespace Application
{
    public class World(Entity player)
    {
        private readonly List<ISystem> _systems = [];
        private readonly Entity _player = player;
        private bool _isAlive = true;

        public bool IsAlive  => _isAlive;
        public Entity Player => _player;

        public void Update() {
            foreach (var system in _systems) {
                system.Update();
            }
        }
    }
}