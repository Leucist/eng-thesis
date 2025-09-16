using Application.Components;

namespace Application.Entities
{
    public class EntityManager
    {
        private uint _lastID;
        private readonly Dictionary<ComponentBitset, Dictionary<Entity, List<Component>>> _entities;

        public EntityManager()
        {
            _lastID = 0;
            _entities = [];
        }

        public Entity CreateEntity()
        {
            Entity entity = new(_lastID++);

            PlaceEntityInGroup(entity, []);

            return entity;
        }

        public List<List<Component>> GetComponentsOfType(List<ComponentType> types) {
            List<List<Component>> result = [];

            foreach(var group in _entities) {
                if (group.Key.HasAll(types)) result.AddRange(group.Value.Values);
            }

            return result;
        }

        private List<Component> GetEntityComponents(Entity entity)
        {
            return _entities[entity.Bitmask][entity];
        }

        private void RemoveEntityFromTheCurrentGroup(Entity entity) {
            _entities[entity.Bitmask].Remove(entity);
            if (_entities[entity.Bitmask].Count == 0) {
                _entities.Remove(entity.Bitmask);   // Removes the group if it became empty
            }
        }

        private void PlaceEntityInGroup(Entity entity, List<Component> components) {
            if (!_entities.ContainsKey(entity.Bitmask)) {
                _entities.Add(entity.Bitmask.Copy, []);  // Adds new empty group dictionary if there was none
                // ^ used Copy to ensure the object will not be modified later by mistake 
            }
            _entities[entity.Bitmask].Add(entity, components);
        }

        public void AddComponent(Entity entity, Component component) {
            // Retrieve components and remove entity from the group
            var components = GetEntityComponents(entity);
            RemoveEntityFromTheCurrentGroup(entity);
            
            // Add component to the entity
            components.Add(component);
            entity.Bitmask.Add(component.Type);
            
            // Place entity and its components into appropriate group
            PlaceEntityInGroup(entity, components);
        }

        public void RemoveComponent(Entity entity, ComponentType componentType) {
            // Retrieve components and check if component is contained
            var components = GetEntityComponents(entity);
            var component = components.FirstOrDefault(c => c.Type == componentType);
            if (component is null) return;  // exit if there's nothing to delete

            // Remove entity from the group
            RemoveEntityFromTheCurrentGroup(entity);
            
            // Remove component from the entity
            components.Remove(component);
            entity.Bitmask.Remove(component.Type);
            
            // Place entity and its components into appropriate group
            PlaceEntityInGroup(entity, components);
        }

        public Entity GetMainMenuPlayer() {
            // TODO May load data from some json prefab and return object
            var player = CreateEntity();

            List<Component> componentsToAdd = [];

            componentsToAdd.Add(new TransformComponent(
                10f, 10f, 10f, 10f  // - values to be changed
            ));

            // componentsToAdd.Add(new PhysicsComponent);
            // componentsToAdd.Add(new CombatComponent);
            // componentsToAdd.Add(new InputComponent);

            foreach (Component component in componentsToAdd) {
                AddComponent(player, component);
            }

            return player;
        }
    }
}