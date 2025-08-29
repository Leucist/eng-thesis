using Application.AppMath;

namespace Application.Components
{
    public class PhysicsComponent : IComponent
    {
        private ForceVector _velocity;              // vector to store value + angle of the force
        private ForceVector _appliedForce, _weight;
        private int _mass, _maxSpeed;

        private bool _massWasChanged = false;   // flag for getting cached P = m * g
        private bool _isFalling;

        public bool IsFalling {
            get { return _isFalling; }
            set { _isFalling = value; } // may be extended as separate func for counting fall damage
        }
        
        private ForceVector Acceleration => CountAcceleration();
        private ForceVector Weight => GetWeight();

        public PhysicsComponent(int mass, int maxSpeed) {
            _mass = mass >= 0 ? mass : throw new ArgumentOutOfRangeException(nameof(mass), "Mass value can not be negative.");
            _maxSpeed = maxSpeed;

            _appliedForce   = ForceVector.Zero;
            _velocity       = ForceVector.Zero;

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

        private ForceVector CountResultingForce() {
            ForceVector resultingForce = ForceVector.Zero;

            resultingForce += _appliedForce;        // counts applied force in final resulting force vector
            _appliedForce.Value /= 2;               // reduces applied force value for decreasing inertia

            if (_isFalling) {                       // if body is airborne, the weight is applied
                resultingForce += Weight;
            }

            return resultingForce;
        }

        private ForceVector CountAcceleration() {
            ForceVector acceleration = CountResultingForce();
            acceleration /= _mass;

            return acceleration;
        }

        
        public ForceVector CountVelocity(int deltatime) {
            _velocity = ForceVector.AddWithCaching(_velocity, Acceleration * deltatime);
            
            if (_velocity.Value > _maxSpeed) _velocity.Value = _maxSpeed;

            return _velocity;
        }

        public void AddAppliedForce(ForceVector force) {
            _appliedForce += force;
        }
    }
}