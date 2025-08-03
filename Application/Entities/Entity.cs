namespace Application.Entities
{
    public readonly struct Entity(int id)
    {
        // Entity may be deprecated to typedef of int if not expanded further
        private readonly int _id = id;

        public int Id => _id;
    }
}