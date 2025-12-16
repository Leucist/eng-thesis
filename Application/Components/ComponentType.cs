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
        Input       = 1 << 4, // ..and so on
        Collision   = 1 << 5,
        Combat      = 1 << 6,
    }
}