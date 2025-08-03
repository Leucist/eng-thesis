namespace Application.Components
{
    public class TransformComponent : IComponent
    {
        private float _x;
        private float _y;
        private float _width;
        private float _height;

        public float X => _x;
        public float Y => _y;
        public float Width => _width;
        public float Height => _height;

        public TransformComponent(float x, float y, float width, float height)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public void Move(float x, float y) {
            _x += x;
            _y += y;
        }
    }
}