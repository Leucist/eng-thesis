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

        protected virtual Dictionary<ComponentType, Component> ParseComponents(List<Component> componentBundle) {
            Dictionary<ComponentType, Component> result = [];
            // Parse the bundle to extract only required components
            foreach (var component in componentBundle) {
                if (_requiredComponents.Contains(component.Type)) {
                    // Add component with the matching key
                    result.Add(component.Type, component);
                }
            }
            return result;
        }

        /// <summary>
        /// Declares basic behavior for Update method by gathering all the required component bundles,
        /// parsing them into Dictionary(ComponentType, Component) and starting the foreach loop 
        /// to perform the actual system logic.
        /// </summary>
        public virtual void Update() {
            var componentBundles = GatherComponents();
            foreach (var componentBundle in componentBundles) {
                PerformSystemAction(ParseComponents(componentBundle));
            }
        }

        protected abstract void PerformSystemAction(Dictionary<ComponentType, Component> entityComponents);
    }
}