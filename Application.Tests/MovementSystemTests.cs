using Application.Systems;
using Application.Components;
using Application.Entities;
using Application.AppMath;

namespace Application.Tests
{
    public class MovementSystemTests
    {
        [Fact]
        public void EntityShouldBeAbleToMove() {
            EntityManager entityManager = new();
            Entity entity = entityManager.CreateEntity();
            MovementSystem system = new(entityManager);
            float x = 10, y = 20;
            TransformComponent  transformComponent  = new(x, y, 10, 20);
            PhysicsComponent    physicsComponent    = new(10, 120);
            entityManager.AddComponent(entity, transformComponent);
            entityManager.AddComponent(entity, physicsComponent);
            
            physicsComponent.AddAppliedForce(new (10, 0));
            system.Update();

            Assert.Equal(y, transformComponent.Y);  // Y stayed the same
            Assert.True(x < transformComponent.X);  // X increased as the entity moved to the right
        }

        [Fact]
        public void EntityShouldBeAbleToJump() {
            EntityManager entityManager = new();
            Entity entity = entityManager.CreateEntity();
            MovementSystem system = new(entityManager);
            float x = 10, y = 20;
            TransformComponent  transformComponent  = new(x, y, 10, 20);
            PhysicsComponent    physicsComponent    = new(10, 120);
            entityManager.AddComponent(entity, transformComponent);
            entityManager.AddComponent(entity, physicsComponent);
            
            physicsComponent.AddAppliedForce(new (10, 90.ToRadians()));
            system.Update();

            Assert.Equal(x, transformComponent.X);  // X stayed the same
            Assert.True(y > transformComponent.Y);  // Y decreased as the entity moved up
        }
    }
}