using System.Numerics;
using Application.Components;
using Application.AppMath;

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
        [InlineData(-1)]
        [InlineData(-85)]
        public void PhysicsComponent_ShouldNotAcceptNegativeMass(int mass) {
            int customMaxSpeed = 35;
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhysicsComponent(mass, customMaxSpeed));
        }

        [Fact]
        public void PhysicsComponent_FallingObjectShouldFall() {
            int mass = 20,
                customMaxSpeed = 3500,
                timeSpan1 = 1,
                timeSpan2 = 5;
            PhysicsComponent component = new(mass, customMaxSpeed);
            // Vector2 weight = new (mass * 9.8f, (float) AngleDirections.Down);
            component.IsFalling = true;

            var v1 = component.CountVelocity(timeSpan1);
            var v2 = component.CountVelocity(timeSpan2);
            
            float expectedV0 = 0f;
            float expectedV1 = expectedV0 + (MathConstants.GravitationalAcceleration * timeSpan1);
            float expectedV2 = expectedV1 + (MathConstants.GravitationalAcceleration * timeSpan2);

            // Are object velocity vectors directed down?
            Assert.Equal(-MathConstants.RadiansUpDirection, v1.Angle, 0.1f);
            Assert.Equal(-MathConstants.RadiansUpDirection, v2.Angle, 0.1f);
            // Are object velocity vectors close to the expected values?
            Assert.Equal((int) expectedV1, v1.Value);
            Assert.Equal((int) expectedV2, v2.Value, 5f);   // rather vague precision as ints are used instead of floats
        }
    }
}