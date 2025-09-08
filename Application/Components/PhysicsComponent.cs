using Application.AppMath;

namespace Application.Components
{
    public class PhysicsComponent : IComponent
    {
        private ForceVector _appliedForce, _weight;
        private int _mass, _maxSpeed;

        private bool _massWasChanged;   // flag for getting cached P = m * g
        private bool _isFalling;

        public bool IsFalling {
            get { return _isFalling; }
            set { _isFalling = value; }
        }
        
        private ForceVector Weight          => GetWeight();
        private ForceVector ResultingForce  => CountResultingForce();
        private ForceVector Acceleration    => CountAcceleration();

        public PhysicsComponent(int mass, int maxSpeed) {
            _mass = mass >= 0 ? mass : throw new ArgumentOutOfRangeException(nameof(mass), "Mass value can not be negative.");
            _maxSpeed = maxSpeed;

            _appliedForce   = ForceVector.Zero;

            _weight.Angle   = MathConstants.RadiansDownDirection;

            _massWasChanged = true;
            _isFalling      = false;
        }

        private ForceVector GetWeight() {
            if (_massWasChanged) {
                _weight.Value = (int) (_mass * MathConstants.GravitationalAcceleration);    // "P = mg"
                _massWasChanged = false;
            }
            return _weight;
        }

        private ForceVector CountResultingForce() { // Resets the aF by adding Weight to it if the body is airborne
            if (_isFalling) {
                _appliedForce += Weight;
            }

            return _appliedForce;
        }

        private ForceVector CountAcceleration() {
            return ResultingForce / _mass;
        }

        
        public ForceVector CountVelocity(int deltatime) {
            ForceVector velocity = Acceleration * deltatime;

            // Reduces applied force value for decreasing inertia
            ReduceAppliedForce(deltatime);
            
            // Ensures speed does not exceed the upper limit
            if (velocity.Value > _maxSpeed) velocity.Value = _maxSpeed;

            return velocity;
        }

        private void Ground() {
            _isFalling = false;
            _appliedForce = ForceVector.Zero;
        }

        private void ReduceAppliedForce(int deltatime) {
            _appliedForce /= (int) MathF.Pow(2, deltatime);
        }

        public void AddAppliedForce(ForceVector force) {
            _appliedForce += force;
        }
    }
}