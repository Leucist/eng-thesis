using Application.Components;
using SFML.System;

namespace Application.Tests
{
    public class PhysicsComponentTests
    {
        [Fact]
        public void PhysicsComponent_ShouldInitParamsCorrectly() {
            Vector2f customVelocity = Vector2f.Zero;
            Vector2f customAcceleration = Vector2f.Zero;
            float mass = 20,
                customFriction = 5,
                customMaxSpeed = 35;
            var component = new PhysicsComponent(mass, customFriction, customMaxSpeed, customVelocity, customAcceleration);

            Assert.Equal(component.Velocity, customVelocity);
            Assert.Equal(component.Acceleration, customAcceleration);
            Assert.Equal(component.Mass, mass);
            // TODO – So use frictionModifier maybe? :P
            Assert.Equal(component.Friction, customFriction);         // may be modified depending on surroundings
            Assert.Equal(component.MaxSpeed, customMaxSpeed);
        }

        [Fact]
        public void PhysicsComponent_ShouldHaveDefaultSpeedAndAccelerationAsZero() {
            Vector2f defaultVelocity = Vector2f.Zero;
            Vector2f defaultAcceleration = Vector2f.Zero;
            float mass = 13.6f;
            var component = new PhysicsComponent(mass);

            Assert.Equal(component.Velocity, defaultVelocity);
            Assert.Equal(component.Acceleration, defaultAcceleration);
        }

        [Theory]
        [InlineData(-1f)]
        [InlineData(-85.2f)]
        [InlineData(-0.001f)]
        public void PhysicsComponent_ShouldNotAcceptNegativeMass(float mass) {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhysicsComponent(mass));
        }
    }
}