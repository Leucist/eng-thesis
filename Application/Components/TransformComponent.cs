namespace Application.Components
{
    public class TransformComponent(float x, float y, float width, float height) : Component(ComponentType.Transform)
    {
        private float _x = x;
        private float _y = y;
        private float _width = width;
        private float _height = height;
        private int _direction = 1;

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;
        public int Direction => _direction;

        public void Move(float x, float y) {
            // Change direction, if needed
            int directionChange = Math.Sign(x);
            if (directionChange != 0) _direction = directionChange;

            // Change coordinates
            _x += x;
            _y += y;
        }

        public void Resize(float width, float height) {
            _width = width;
            _height = height;
        }
    }
}