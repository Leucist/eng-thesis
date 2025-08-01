namespace Application.Tests;

public class EntityManagerTests
{
    [Fact]
    public void CreateEntity_ShouldReturnUniqueIds()
    {
        var entityManager = EntityManager.Instance;
        
        var entity1 = entityManager.CreateEntity();
        var entity2 = entityManager.CreateEntity();
        
        Assert.NotEqual(entity1, entity2);
    }

    [Fact]
    public void CreateEntity_EntitiesShouldBeContained()
    {
        var entityManager = EntityManager.Instance;
        
        var entity1 = entityManager.CreateEntity();
        var entity2 = entityManager.CreateEntity();
        
        Assert.True(entityManager.EntityExists(entity1));
        Assert.True(entityManager.EntityExists(entity2));
    }

    // [Fact]
    // public void AddComponent_ShouldStoreAndRetrieveComponent()
    // {
    //     var entityManager = new EntityManager();
    //     var entity = entityManager.CreateEntity();
    //     var position = new PositionComponent(10f, 20f);
        
    //     entityManager.AddComponent(entity, position);
    //     var retrieved = entityManager.GetComponent<PositionComponent>(entity);
        
    //     Assert.Equal(position, retrieved);
    //     Assert.True(entityManager.HasComponent<PositionComponent>(entity));
    // }
}