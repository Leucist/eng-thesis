using Application.Components;

namespace Application.Tests
{
    public class PhysicsComponentTests
    {
        [Fact]
        public void PhysicsComponent_ShouldHaveDefaults() {
        // public void PhysicsComponent_ShouldInitParamsCorrectly() {
            // Probably won't use defaults for long, so test may be changed to just cheking if params set correctly
            float mass = 20;
            var component = new PhysicsComponent(mass);

            Assert.Equal(component.Velocity, PhysicsComponent.DefaultVelocity);             // <- Vector2 or Vector2f
            Assert.Equal(component.Acceleration, PhysicsComponent.DefaultAcceleration);     // <- *same*
            Assert.Equal(component.Mass, mass);
            // TODO – So use frictionModifier maybe? :P
            Assert.Equal(component.Friction, PhysicsComponent.DefaultFriction);         // may be modified depending on surroundings
            Assert.Equal(component.MaxSpeed, PhysicsComponent.DefaultMaxSpeed);
        }
    }
}