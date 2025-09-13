using Application.Entities;
using Application.Components;

namespace Application.Tests;

public class EntityManagerTests
{
    private readonly EntityManager _entityManager = EntityManager.Instance;

    [Fact]
    public void CreateEntity_ShouldReturnUniqueIds()
    {
        var entity1 = _entityManager.CreateEntity();
        var entity2 = _entityManager.CreateEntity();
        
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    [Fact]
    public void AddAndRemoveComponent_ShouldChangeEntityBitmask()
    {
        var entity = _entityManager.CreateEntity();
        var transform = new TransformComponent(10f, 20f, 30f, 40f);
        
        _entityManager.AddComponent(entity, transform);
        Assert.True(entity.Bitmask.Has(ComponentType.Transform));
        _entityManager.RemoveComponent(entity, ComponentType.Transform);
        Assert.False(entity.Bitmask.Has(ComponentType.Transform));
    }

    // - Related tests, but not on EntityManager directly
    [Fact]
    public void ComponentBitset_ShouldIndicatePresenceOfAddedComponents()
    {
        ComponentBitset mask = new();

        mask.Add(ComponentType.Transform);
        mask.Add(ComponentType.Physics);
        
        Assert.True(mask.Has(ComponentType.Transform));
        Assert.True(mask.Has(ComponentType.Physics));
    }

    [Fact]
    public void Entity_BitmaskShouldIndicatePresenceOfAddedComponents()
    {
        Entity entity = new(0);

        entity.Bitmask.Add(ComponentType.Transform);
        entity.Bitmask.Add(ComponentType.Physics);
        
        Assert.True(entity.Bitmask.Has(ComponentType.Transform));
        Assert.True(entity.Bitmask.Has(ComponentType.Physics));
    }
}