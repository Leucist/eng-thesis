using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public abstract class System
    {
        protected readonly EntityManager _entityManager;
        protected readonly List<ComponentType> _requiredComponents;

        protected System(EntityManager entityManager, List<ComponentType> requiredComponents) {
            _entityManager = entityManager;
            _requiredComponents = requiredComponents;
        }

        protected virtual List<List<Component>> GatherComponents() {
            return _entityManager.GetComponentsOfType(_requiredComponents);
        }

        public abstract void Update();
    }
}