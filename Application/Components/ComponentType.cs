namespace Application.Components
{
    [Flags]
    public enum ComponentType
    {
        None        = 0,
        Transform   = 1 << 0, // 0001
        Physics     = 1 << 1, // 0010
        Graphics    = 1 << 2, // 0100
        Animation   = 1 << 3, // 1000
    }
}