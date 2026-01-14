using Application.Components;
using Application.AppMath;

namespace Application.Tests
{
    [Collection("UsingMathCache")]  // Using "Collection" for sequential performing of the tests, as MathCache is not explicitly thread-safe at the moment
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

            float expectedOffset = -(MathConstants.GravitationalAcceleration * timeSpan1 / AppConstants.GRAVITY_SCALE_DIVIDER);

            // - Act
            var mo1 = component.GetMovementOffset(timeSpan1);
            var mo2 = component.GetMovementOffset(timeSpan2);

            // - Assert
            Assert.Equal(expectedOffset, mo1.Y, 1f);    // Is offset of the falling object close to the expected value?
            Assert.True(mo2.Y < mo1.Y);                 // Check that the velocity is increasing
        }

        [Fact]
        public void PhysicsComponent_NotFallingObjectShouldStayIdle() {
            // - Arrange
            int mass = 20,
                customMaxSpeed = 3500,
                timeSpan1 = 1,
                timeSpan2 = 5;
            PhysicsComponent component = new(mass, customMaxSpeed);

            // - Act
            var mo1 = component.GetMovementOffset(timeSpan1);
            var mo2 = component.GetMovementOffset(timeSpan2);

            // - Assert
            Assert.Equal(0f, mo1.X);
            Assert.Equal(0f, mo1.Y);

            Assert.Equal(0f, mo2.X);
            Assert.Equal(0f, mo2.Y);
        }

        [Fact]
        public void PhysicsComponent_ShouldMoveWhenForceIsApplied()
        {
            // - Arrange
            int mass = 10;
            int maxSpeed = 100;
            PhysicsComponent physicsComponent = new (mass, maxSpeed);
            ForceVector appliedForce = new (50, MathConstants.RadiansRightDirection);   // Force directed to the right
            int deltaTime = 1;
            float instantSpeed = appliedForce.Value / mass;

            // - Act
            physicsComponent.AddAppliedForce(appliedForce);
            OffsetEntry movementOffset = physicsComponent.GetMovementOffset(deltaTime);

            // - Assert
            Assert.True(movementOffset.X > 0, "X offset should be greater than 0 when horizontal force is applied.");
            Assert.Equal(instantSpeed, movementOffset.X, 0.1f);  // Check that the velocity corresponds to the acceleration.
        }

        [Fact]
        public void PhysicsComponent_ShouldStopWhenInertiaIsGone()
        {
            // - Arrange
            int mass = 10;
            int maxSpeed = 100;
            PhysicsComponent physicsComponent = new (mass, maxSpeed);
            ForceVector appliedForce = new (20, MathConstants.RadiansRightDirection);
            int deltaTime = 1;

            // - Act
            physicsComponent.AddAppliedForce(appliedForce);     // Apply force

            var mo1 = physicsComponent.GetMovementOffset(deltaTime); // Count velocity
            var mo2 = physicsComponent.GetMovementOffset(deltaTime); // Count velocity again to check fading
            var mo3 = physicsComponent.GetMovementOffset(deltaTime); // Count velocity to ensure it's zero

            // - Assert
            Assert.True(mo1.X > mo2.X, "Velocity should decrease over time when no new force is applied.");
            Assert.Equal((0, 0), (mo3.X, mo3.Y));
        }

        [Fact]
        public void PhysicsComponent_ShouldNotReachSpeedBeyondLimit()
        {
            // - Arrange
            int mass = 10;
            int maxSpeed = 100;
            PhysicsComponent physicsComponent = new (mass, maxSpeed);
            ForceVector appliedForce = new (5000, MathConstants.RadiansRightDirection);
            int deltaTime = 1;
            int instantSpeed = appliedForce.Value / mass;

            // - Act
            physicsComponent.AddAppliedForce(appliedForce);         // Apply force
            var mo1 = physicsComponent.GetMovementOffset(deltaTime);

            // - Assert
            Assert.True(instantSpeed > maxSpeed, "[Test Requirement] Applied force should be enough to potentially make instant speed higher than the 'maxSpeed'.");
            Assert.True(mo1.X < instantSpeed, "Actual velocity should be lower than it potentially could be based on formula.");
            Assert.Equal(maxSpeed, mo1.X);   // as velocity would be reduced to match the max allowed value
        }
    }
}