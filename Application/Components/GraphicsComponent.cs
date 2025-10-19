using SFML.Graphics;

namespace Application.Components
{
    public class GraphicsComponent() : Component(ComponentType.Graphics)
    {
        // TODO: Ensure fields won't stay null while exiting the constructor
        private Texture _texture;
        private Sprite _sprite;
        private bool _textureChanged = true;

        public Sprite Sprite => GetSprite();

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