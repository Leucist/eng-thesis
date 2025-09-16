namespace Application.Components
{
    [Flags]
    public enum ComponentType
    {
        None        = 0,
        Transform   = 1 << 0, // 1
        Physics     = 1 << 1, // 2
    }
}