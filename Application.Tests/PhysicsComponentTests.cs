using System.Numerics;
using Application.Components;
using Application.Enums;

namespace Application.Tests
{
    public class PhysicsComponentTests
    {
        // [Fact]
        // public void PhysicsComponent_ShouldInitParamsCorrectly() {
        //     Vector2 customVelocity = new(100f, 2.5f);
        //     float mass = 20,
        //         customMaxSpeed = 35;
        //     var component = new PhysicsComponent(mass, customMaxSpeed);

        //     Assert.Equal(component.Velocity, customVelocity);
        //     Assert.Equal(component.Acceleration, customAcceleration);
        //     Assert.Equal(component.Mass, mass);
        //     Assert.Equal(component.MaxSpeed, customMaxSpeed);
        // }

        // [Fact]
        // public void PhysicsComponent_ShouldHaveDefaultSpeedAndAccelerationAsZero() {
        //     Vector2 defaultVelocity = Vector2.Zero;
        //     Vector2 defaultAcceleration = Vector2.Zero;
        //     float mass = 13.6f;
        //     var component = new PhysicsComponent(mass);

        //     Assert.Equal(component.Velocity, defaultVelocity);
        //     Assert.Equal(component.Acceleration, defaultAcceleration);
        // }

        [Theory]
        [InlineData(-1f)]
        [InlineData(-85.2f)]
        [InlineData(-0.001f)]
        public void PhysicsComponent_ShouldNotAcceptNegativeMass(float mass) {
            float customMaxSpeed = 35f;
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhysicsComponent(mass, customMaxSpeed));
        }

        // [Fact]
        // public void PhysicsComponent_ShouldNotAcceptVelocityGreaterThanMaxSpeed() {
        //     float maxSpeed = 42.7f;
        //     float moreThanMaxSpeed = maxSpeed + 0.1f;
        //     Vector2 currentVelocity = new(moreThanMaxSpeed, 0f);

        //     Assert.Throws<ArgumentOutOfRangeException>(() => new PhysicsComponent(25, maxSpeed: maxSpeed, velocity: currentVelocity));
        // }

        [Fact]
        public void PhysicsComponent_FallingObjectShouldFall() {
            float mass = 20,
                customMaxSpeed = 3500,
                timeSpan1 = 1f,
                timeSpan2 = 5f;
            const float gravitationalAcceleration = 9.8f;
            PhysicsComponent component = new(mass, customMaxSpeed);
            // Vector2 weight = new (mass * 9.8f, (float) AngleDirections.Down);
            component.IsFalling = true;

            var v1 = component.CountVelocity(timeSpan1);
            var v2 = component.CountVelocity(timeSpan2);
            
            float expectedV0 = 0f;
            float expectedV1 = expectedV0 + (gravitationalAcceleration * timeSpan1);
            float expectedV2 = expectedV1 + (gravitationalAcceleration * timeSpan2);

            // Are object velocity vectors directed down?
            Assert.Equal((float)AngleDirections.Down, v1.Y, 0.1f);
            Assert.Equal((float)AngleDirections.Down, v2.Y, 0.1f);
            // Are object velocity vectors close to the expected values?
            Assert.Equal(expectedV1, v1.X, 0.1f);
            Assert.Equal(expectedV2, v2.X, 0.1f);
        }
    }
}