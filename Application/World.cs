using Application.Systems;

namespace Application
{
    public class World
    {
        private List<ISystem> _systems;
        private bool _isAlive;

        public bool IsAlive => _isAlive;

        public World() {
            _systems = [];
            _isAlive = true;
        }

        public void Update() {
            foreach (var system in _systems) {
                system.Update();
            }
        }
    }
}