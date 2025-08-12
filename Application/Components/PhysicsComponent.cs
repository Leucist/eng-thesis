using System.Numerics;
using Application.Enums;

namespace Application.Components
{
    public class PhysicsComponent : IComponent
    {
        private Vector2 _velocity;              // vector to store value + angle of the force
        private Vector2 _appliedForce, _weight;
        private float _mass, _maxSpeed;

        private bool _massWasChanged = false;   // flag for getting cached P = m * g
        private bool _isFalling;

        public bool IsFalling {
            get { return _isFalling; }
            set { _isFalling = value; } // may be extended as separate func for counting fall damage
        }
        
        private Vector2 Acceleration => CountAcceleration();
        private Vector2 Weight => GetWeight();

        public PhysicsComponent(float mass, float maxSpeed) {
            _mass = mass >= 0 ? mass : throw new ArgumentOutOfRangeException(nameof(mass), "Mass value can not be negative.");
            _maxSpeed = maxSpeed;

            _appliedForce   = Vector2.Zero;
            _velocity       = Vector2.Zero;

            _weight.Y = (float) AngleDirections.Down;

            _massWasChanged = true;
            _isFalling      = false;
        }

        private Vector2 GetWeight() {
            if (_massWasChanged) {
                _weight.X = _mass * 9.8f;   // 9.8f stands for 'g' in "P = mg"
                _massWasChanged = false;
            }
            return _weight;
        }

        private Vector2 CountResultingForce() {
            Vector2 resultingForce = Vector2.Zero;

            resultingForce += _appliedForce;    // counts applied force in final resulting force vector
            _appliedForce.X /= 2;               // reduces applied force value for decreasing inertia
            if (_appliedForce.X < 1) _appliedForce.X = 0;

            if (_isFalling) {                   // if body is airborne, the weight is applied
                resultingForce += Weight;
            }

            return resultingForce;
        }

        private Vector2 CountAcceleration() {
            Vector2 acceleration = CountResultingForce();
            acceleration.X /= _mass;

            return acceleration;
        }

        
        public Vector2 CountVelocity(float deltatime) {
            _velocity += new Vector2(Acceleration.X * deltatime, Acceleration.Y);
            
            if (_velocity.X > _maxSpeed) _velocity.X = _maxSpeed;
            if (_velocity.X < 0) _velocity = Vector2.Zero;          // resets velocity if it crosses zero in value

            return _velocity;
        }

        public void AddAppliedForce(Vector2 force) {
            _appliedForce += force;
        }
    }
}