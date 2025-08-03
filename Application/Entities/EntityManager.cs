using Application.Components;

namespace Application.Entities
{
    public class EntityManager
    {
        private static EntityManager _instance = new();
        private static readonly object _lock = new();   // Lock object for thread-safe singleton instantiation

        private int _lastID;
        private readonly Dictionary<Entity, List<IComponent>> _entities;

        private EntityManager()
        {
            _lastID = 0;
            _entities = [];
        }

        public static EntityManager Instance
        {
            get
            {
                // Double-check locking for thread safety
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new EntityManager();
                    }
                }
                return _instance;
            }
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(_lastID++);
            _entities.Add(entity, []);
            return entity;
        }

        public bool EntityExists(Entity entity)
        {
            return _entities.ContainsKey(entity);
        }

        public void AddComponent(Entity entity, IComponent component) {
            if (!EntityExists(entity)) throw new Exception("Entity does not exist.");
            _entities[entity].Add(component);
        }

        public void RemoveComponent<T>(Entity entity) where T : IComponent {
            if (!EntityExists(entity)) throw new Exception("Entity does not exist.");

            var componentToRemove = _entities[entity].FirstOrDefault(c => c is T);
    
            if (componentToRemove != null) {
                _entities[entity].Remove(componentToRemove);
            }
        }

        public bool HasComponent<T>(Entity entity) {
            if (!EntityExists(entity)) throw new Exception("Entity does not exist.");
            return _entities[entity].Any(c => c is T);
        }

        public T GetComponent<T>(Entity entity) {
            if (!EntityExists(entity)) throw new Exception("Entity does not exist.");
            var component = _entities[entity].FirstOrDefault(c => c is T) ?? throw new Exception($"Component of type {typeof(T)} does not exist for the entity.");
            return (T)component;
        }
    }
}