namespace Application.Components
{
    public class ComponentBitset(uint bits=0)
    {
        private uint _bits = bits;

        public ComponentBitset Copy => new(_bits);

        public void Add(ComponentType component)
        {
            _bits |= (uint)component;
        }

        public void Remove(ComponentType component)
        {
            _bits &= ~(uint)component;
        }

        public bool Has(ComponentType component)
        {
            return (_bits & (uint)component) != 0;
        }

        public bool HasAll(List<ComponentType> components) => components.All(Has);

        // public bool HasAll(List<ComponentType> components)
        // {
        //     foreach (ComponentType c in components) {
        //         if (!Has(c)) return false;
        //     }
        //     return true;
        // }

        // TODO: May be updated for better performance. For now it stays this way.
        public override int GetHashCode()
        {
            return _bits.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is ComponentBitset otherBitset) {
                return _bits == otherBitset._bits;
            }
            return false;
        }

        // * Overload allows to avoid unnecessary type casting
        public bool Equals(ComponentBitset other) {
            return _bits == other._bits;
        }
    }
}