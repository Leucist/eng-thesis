using SFML.Graphics;

namespace Application
{
    public class GraphicsCache
    {
        private readonly Dictionary<string, Texture> _texturesCache;
        private static GraphicsCache? _instance;

        private GraphicsCache() {
            _texturesCache = [];

            // * Pre-load the defined textures.

            // Load the Background  -> Pathfinder.GetGraphicsFolder / BG

            // Load all textures    -> Pathfinder.GetTexturesFolder

            // TODO Probably would be a good idea to make this cache Game-wide, all in all. As the textures are same for now.

            foreach (var path in Pathfinder.GetAllTexturePaths()) {
                _texturesCache.Add(path, new Texture(path));
            }
        }

        public Texture GetTexture(string path) {
            if (!_texturesCache.TryGetValue(path, out Texture? requestedTexture)) {
                requestedTexture = new Texture(Pathfinder.GetFullTextureFilePath(path));
                _texturesCache[path] = requestedTexture;
            }
            return requestedTexture;
        }

        public static Texture GetTextureFromCache(string path) {
            _instance ??= new GraphicsCache();
            return _instance.GetTexture(path);
        }
    }
}