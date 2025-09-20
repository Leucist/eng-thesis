using SFML.System;

namespace Application.Components
{
    public class TransformComponent(float x, float y, float width, float height) : Component(ComponentType.Transform)
    {
        private float _x = x;
        private float _y = y;
        private float _width = width;
        private float _height = height;

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;

        /// <summary>
        /// Special for convenient interacting with the SFML via utilizing its type
        /// </summary>
        public Vector2f SFMLPosition => new(_x, _y);

        public void Move(float x, float y) {
            _x += x;
            _y += y;
        }

        public void Resize(float width, float height) {
            _width = width;
            _height = height;
        }
    }
}