using Application.Components;
using Application.Systems;

namespace Application.Worlds
{
    public class WorldDTO
    {
        public string Background;
        public int TileWidth;
        public int TileHeight;
        public List<List<Component>> Entities;
        public List<ASystem> Systems;
    }
}