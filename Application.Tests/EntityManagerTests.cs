using Application.Entities;
using Application.Components;

namespace Application.Tests;

public class EntityManagerTests
{
    private readonly EntityManager _entityManager = EntityManager.Instance;
    private readonly Entity _entity = EntityManager.Instance.CreateEntity();

    [Fact]
    public void CreateEntity_ShouldReturnUniqueIds()
    {
        var entity2 = _entityManager.CreateEntity();
        
        Assert.NotEqual(_entity, entity2);
    }

    [Fact]
    public void CreateEntity_EntitiesShouldBeContained()
    {
        var entity2 = _entityManager.CreateEntity();
        
        Assert.True(_entityManager.EntityExists(_entity));
        Assert.True(_entityManager.EntityExists(entity2));
    }

    [Fact]
    public void AddComponent_ShouldStoreAndRetrieveComponent()
    {
        var transform = new TransformComponent(10f, 20f, 30f, 40f);
        
        _entityManager.AddComponent(_entity, transform);
        var retrieved = _entityManager.GetComponent<TransformComponent>(_entity);
        
        Assert.Equal(transform, retrieved);
        Assert.True(_entityManager.HasComponent<TransformComponent>(_entity));
    }

    [Fact]
    public void RemoveComponent_ShouldRemoveComponent()
    {        
        _entityManager.RemoveComponent<TransformComponent>(_entity);
        
        Assert.False(_entityManager.HasComponent<TransformComponent>(_entity));
    }
}