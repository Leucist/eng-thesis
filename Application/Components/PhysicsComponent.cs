using SFML.System;

namespace Application.Components
{
    public class PhysicsComponent : IComponent
    {
        private readonly Vector2f _velocity, _acceleration;
        private float _mass, _friction, _maxSpeed;
        
        private static Vector2f DEFAULT_VELOCITY     = new(0, 0);
        private static Vector2f DEFAULT_ACCELERATION = new(0, 0);

        // * Pretty Getters ✨ *
        public float Mass               => _mass;
        public float Friction           => _friction;
        public float MaxSpeed           => _maxSpeed;
        public Vector2f Velocity        => _velocity;
        public Vector2f Acceleration    => _acceleration;
        // **

        public PhysicsComponent(float mass, float friction=0f, float maxSpeed=30f, 
                                Vector2f? velocity=null, Vector2f? acceleration=null) {
            _mass = mass >= 0 ? mass : throw new ArgumentOutOfRangeException(nameof(mass), "Mass value can not be negative.");
            _friction = friction;
            _maxSpeed = maxSpeed;
            _velocity = velocity ?? DEFAULT_VELOCITY;
            _acceleration = acceleration ?? DEFAULT_ACCELERATION;
        }
    }
}