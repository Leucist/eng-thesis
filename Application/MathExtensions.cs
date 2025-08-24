namespace Application
{
    public static class MathExtensions
    {
        public static float ToRadians(this int degrees) {
            return degrees * MathF.PI / 180f;
        }
    }
}