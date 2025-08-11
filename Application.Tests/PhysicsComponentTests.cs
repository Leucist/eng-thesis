using System.Numerics;
using Application.Components;

namespace Application.Tests
{
    public class PhysicsComponentTests
    {
        [Fact]
        public void PhysicsComponent_ShouldInitParamsCorrectly() {
            Vector2 customVelocity = new(100f, 2.5f);
            float mass = 20,
                customMaxSpeed = 35;
            var component = new PhysicsComponent(mass, customMaxSpeed, customVelocity, customAcceleration);

            Assert.Equal(component.Velocity, customVelocity);
            Assert.Equal(component.Acceleration, customAcceleration);
            Assert.Equal(component.Mass, mass);
            Assert.Equal(component.MaxSpeed, customMaxSpeed);
        }

        [Fact]
        public void PhysicsComponent_ShouldHaveDefaultSpeedAndAccelerationAsZero() {
            Vector2 defaultVelocity = Vector2.Zero;
            Vector2 defaultAcceleration = Vector2.Zero;
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

        [Fact]
        public void PhysicsComponent_ShouldNotAcceptVelocityGreaterThanMaxSpeed() {
            float maxSpeed = 42.7f;
            float moreThanMaxSpeed = maxSpeed + 0.1f;
            Vector2f currentVelocity = new(moreThanMaxSpeed, 0f);

            Assert.Throws<ArgumentOutOfRangeException>(() => new PhysicsComponent(25, maxSpeed: maxSpeed, velocity: currentVelocity));
        }

        [Fact]
        public void PhysicsComponent_ShouldKeepAnglesInRange() {
            Vector2f customVelocity = new(-100f, -2500f);
            Vector2f customAcceleration = new(250f, 360f);

            float correctVAngle = ;
            float correctAAngle = 0;

            float mass = 20;
            var component = new PhysicsComponent(mass, velocity: customVelocity, acceleration: customAcceleration);

            Assert.Equal(component.Velocity.X, customVelocity.X);   // Value stays the same
            Assert.Equal(component.Velocity.Y, customVelocity.Y);   // Angle gets changed

            Assert.Equal(component.Acceleration.X, customAcceleration.X);
            Assert.Equal(component.Acceleration.Y, customAcceleration.Y);
        }
    }
}