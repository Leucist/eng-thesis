namespace Application.Entities
{
    public readonly struct Entity(int id)
    {
        private readonly int _id = id;
    }
}