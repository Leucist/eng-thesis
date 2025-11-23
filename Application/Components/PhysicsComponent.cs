using Application.AppMath;

namespace Application.Components
{
    public class PhysicsComponent : Component
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

        // * For JSON (de-)serialization
        public int Mass => _mass;
        public int MaxSpeed => _maxSpeed;
        public ForceVector AppliedForce => _appliedForce;

        public PhysicsComponent(int mass, int maxSpeed) : base(ComponentType.Physics) {
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

        private void ReduceAppliedForce(int deltatime) {
            _appliedForce /= (int) MathF.Pow(2, deltatime);
        }

        public void AddAppliedForce(ForceVector force) {
            _appliedForce += force;
        }

        private ForceVector CountVelocity(int deltatime) {
            ForceVector velocity = Acceleration * deltatime;

            // Reduces applied force value for decreasing inertia
            ReduceAppliedForce(deltatime);
            
            // Ensures speed does not exceed the upper limit
            if (velocity.Value > _maxSpeed) velocity.Value = _maxSpeed;

            return velocity;
        }

        public OffsetEntry GetMovementOffset(int deltatime) {
            return CountVelocity(deltatime).GetOffset();
        }

        public void Ground() {
            _isFalling = false;
            Stop();
        }

        public void Stop() => _appliedForce = ForceVector.Zero;
    }
}