using Application.Components;
using Application.Entities;

namespace Application
{
    public class World(/*List<Component> playerComponents*/)
    {
        private readonly List<Systems.System> _systems = [];
        // private readonly Entity _player = player;
        private bool _isAlive = true;

        public bool IsAlive  => _isAlive;
        // public Entity Player => _player;

        public void Update() {
            foreach (var system in _systems) {
                system.Update();
            }
        }
    }
}