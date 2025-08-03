namespace Application.Entities
{
    public class EntityManager
    {
        private static EntityManager _instance = new();
        private static readonly object _lock = new();   // Lock object for thread-safe singleton instantiation

        private int _lastID;
        private List<Entity> _entities;

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
            _entities.Add(entity);
            return entity;
        }

        public bool EntityExists(Entity entity)
        {
            return _entities.Contains(entity);
        }
    }
}