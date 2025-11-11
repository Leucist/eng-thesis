using SFML.Graphics;

namespace Application.Components
{
    public class GraphicsComponent : Component
    {
        private Texture _texture;
        private Sprite _sprite;
        private bool _textureChanged;
        private string _texturePath;    // for storing path of the initial image in JSON

        public Sprite Sprite => GetSprite();
        public string TexturePath => _texturePath;

        public GraphicsComponent(string texturePath) : base(ComponentType.Graphics) {
            _texturePath = texturePath;
            // Sets values of the _texture and _textureChanged flag
            SetTexture(texturePath);
        }

        // * pathToImage being ~"<EntityType>/<AnimationType>/<Image>"
        public void SetTexture(string pathToImage) {
            _texture = GraphicsCache.GetTextureFromCache(Pathfinder.GetFullTextureFilePath(pathToImage));
            _textureChanged = true; // resets the flag that the Sprite must be updated
        }

        public Sprite GetSprite() {
            if (_textureChanged) {
                _sprite = new(_texture);
                _textureChanged = false;
            }
            return _sprite;
        }
    }
}