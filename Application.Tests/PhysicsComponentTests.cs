using System.Numerics;
using Application.Components;
using Application.AppMath;

namespace Application.Tests
{
    public class PhysicsComponentTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(-85)]
        public void PhysicsComponent_ShouldNotAcceptNegativeMass(int mass) {
            int customMaxSpeed = 35;
            Assert.Throws<ArgumentOutOfRangeException>(() => new PhysicsComponent(mass, customMaxSpeed));
        }

        [Fact]
        public void PhysicsComponent_FallingObjectShouldFall() {
            // - Arrange
            int mass = 20,
                customMaxSpeed = 3500,
                timeSpan1 = 1,
                timeSpan2 = 5;
            PhysicsComponent component = new(mass, customMaxSpeed);
            component.IsFalling = true;

            float expectedV0 = 0f;
            float expectedV1 = expectedV0 + (MathConstants.GravitationalAcceleration * timeSpan1);
            float expectedV2 = expectedV1 + (MathConstants.GravitationalAcceleration * timeSpan2);

            // - Act
            var v1 = component.CountVelocity(timeSpan1);
            var v2 = component.CountVelocity(timeSpan2);

            // - Assert
            // Are object velocity vectors directed down?
            Assert.Equal(-MathConstants.RadiansUpDirection, v1.Angle, 0.1f);
            Assert.Equal(-MathConstants.RadiansUpDirection, v2.Angle, 0.1f);
            // Are object velocity vectors close to the expected values?
            Assert.Equal((int) expectedV1, v1.Value);
            Assert.Equal((int) expectedV2, v2.Value, 5f);   // rather vague precision as ints are used instead of floats
        }

        [Fact]
        public void PhysicsComponent_NotFallingObjectShouldStayIdle() {
            // - Arrange
            int mass = 20,
                customMaxSpeed = 3500,
                timeSpan1 = 1,
                timeSpan2 = 5;
            PhysicsComponent component = new(mass, customMaxSpeed);
            int expectedV = 0;

            // - Act
            var v1 = component.CountVelocity(timeSpan1);
            var v2 = component.CountVelocity(timeSpan2);

            // - Assert
            Assert.Equal(0f, v1.Angle);
            Assert.Equal(0f, v2.Angle);
            Assert.Equal(expectedV, v1.Value);
            Assert.Equal(expectedV, v2.Value);
        }

        [Fact]
        public void PhysicsComponent_ShouldMoveWhenForceIsApplied()
        {
            // - Arrange
            int mass = 10;
            int maxSpeed = 100;
            PhysicsComponent physicsComponent = new (mass, maxSpeed);
            ForceVector appliedForce = new (50, 0);   // Force directed to the right
            int deltaTime = 1;
            int instantSpeed = appliedForce.Value / mass;

            // - Act
            physicsComponent.AddAppliedForce(appliedForce);
            ForceVector newVelocity = physicsComponent.CountVelocity(deltaTime);

            // - Assert
            Assert.True(newVelocity.Value > 0, "Velocity should be greater than 0 when force is applied.");
            Assert.Equal(instantSpeed, newVelocity.Value);  // Check that the velocity corresponds to the acceleration.
        }

        [Fact]
        public void PhysicsComponent_ShouldStopWhenInertiaIsGone()
        {
            // - Arrange
            int mass = 10;
            int maxSpeed = 100;
            PhysicsComponent physicsComponent = new (mass, maxSpeed);
            ForceVector appliedForce = new (20, 0);
            int deltaTime = 1;

            // - Act
            physicsComponent.AddAppliedForce(appliedForce);     // Apply force

            var v1 = physicsComponent.CountVelocity(deltaTime); // Count velocity
            var v2 = physicsComponent.CountVelocity(deltaTime); // Count velocity again to check fading
            var v3 = physicsComponent.CountVelocity(deltaTime); // Count velocity to ensure it's zero

            // - Assert
            Assert.True(v1.Value > v2.Value, "Velocity should decrease over time when no new force is applied.");
            Assert.Equal(ForceVector.Zero, v3);
        }

        [Fact]
        public void PhysicsComponent_ShouldNotReachSpeedBeyondLimit()
        {
            // - Arrange
            int mass = 10;
            int maxSpeed = 100;
            PhysicsComponent physicsComponent = new (mass, maxSpeed);
            ForceVector appliedForce = new (5000, 0);
            int deltaTime = 1;
            int instantSpeed = appliedForce.Value / mass;

            // - Act
            physicsComponent.AddAppliedForce(appliedForce);     // Apply force
            var v1 = physicsComponent.CountVelocity(deltaTime);

            // - Assert
            Assert.True(instantSpeed > maxSpeed, "[Test Requirement] Applied force should be enough to potentially make instant speed higher than the 'maxSpeed'.");
            Assert.True(v1.Value < instantSpeed, "Actual velocity should be lower than it potentially could be based on formula.");
            Assert.Equal(maxSpeed, v1.Value);   // as velovity would be reduced to match the max allowed value
        }
    }
}