using Application.Components;
using Application.Entities;

namespace Application.Systems
{
    public abstract class System(EntityManager entityManager)
    {
        protected readonly EntityManager _entityManager = entityManager;
        protected readonly List<ComponentType> _requiredComponents = [];

        protected virtual List<List<Component>> GatherComponents() {
            return _entityManager.GetComponentsOfType(_requiredComponents);
        }

        public abstract void Update();
    }
}