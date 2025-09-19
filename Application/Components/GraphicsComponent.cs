using SFML.Graphics;

namespace Application.Components
{
    public class GraphicsComponent() : Component(ComponentType.Graphics)
    {
        public Texture  Texture { get; private set; }
        public Sprite   Sprite  { get; private set; }

        public void SetTexture(string pathToImage) {
            Texture = new Texture(Pathfinder.GetFullTextureFilePath(pathToImage));
        }
    }
}