using Application.Components;
using Application.Systems;

namespace Application.Worlds
{
    public class WorldDTO
    {
        // public string Background;
        // public int TileWidth;
        // public int TileHeight;
        public required List<List<Component>> Entities { get; set; }
        public required List<string> SystemTypes { get; set; }
    }
}