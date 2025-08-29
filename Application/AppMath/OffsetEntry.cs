namespace Application.AppMath
{
    public struct OffsetEntry(ForceVector forceA, ForceVector forceB, float xOffset, float yOffset)
    {
        public ForceVector ForceA = forceA;
        public ForceVector ForceB = forceB;

        public float X = xOffset;
        public float Y = yOffset;
    }
}