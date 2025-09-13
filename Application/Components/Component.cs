namespace Application.Components
{
    public abstract class Component(ComponentType type)
    {
        public ComponentType Type { get; } = type;
    }
}