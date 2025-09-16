using Application.Components;

namespace Application.Entities
{
    public class Entity(uint id)
    {
        private readonly uint _id = id;
        private readonly ComponentBitset _bitmask = new();

        public uint Id => _id;
        public ComponentBitset Bitmask => _bitmask;
    }
}