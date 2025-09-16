using Application.Entities;
using Application.Components;

namespace Application.Tests;

public class EntityManagerTests
{
    [Fact]
    public void CreateEntity_ShouldReturnUniqueIds()
    {
        EntityManager entityManager = new();

        var entity1 = entityManager.CreateEntity();
        var entity2 = entityManager.CreateEntity();
        
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    [Fact]
    public void AddAndRemoveComponent_ShouldChangeEntityBitmask()
    {
        EntityManager entityManager = new();

        var entity = entityManager.CreateEntity();
        var transform = new TransformComponent(10f, 20f, 30f, 40f);
        
        entityManager.AddComponent(entity, transform);
        Assert.True(entity.Bitmask.Has(ComponentType.Transform));
        entityManager.RemoveComponent(entity, ComponentType.Transform);
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