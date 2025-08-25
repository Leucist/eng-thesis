namespace Application.AppMath
{
    public static class MathCache
    {
        // - Constants -
        public const float RadiansRightDirection = 0f;
        public const float RadiansUpDirection    = 1.57f;
        public const float RadiansLeftDirection  = 3.14f;
        public const float RadiansDownDirection  = 4.71f;
        // -

        private static Dictionary<int, float> _degreeRadianPairs        = [];
        private static Dictionary<float, float> _sinCache               = [];
        private static Dictionary<float, float> _cosCache               = [];
        private static Dictionary<(float, float), float> _atan2Cache    = [];

        public static float GetRadianValue(int degree) {
            if (!_degreeRadianPairs.TryGetValue(degree, out var radian)) {
                radian = degree.ToRadians();
                _degreeRadianPairs[degree] = radian;
            }
            return radian;
        }

        public static float GetSin(float angle)
        {
            if (!_sinCache.TryGetValue(angle, out float sinValue))
            {
                sinValue = MathF.Sin(angle);
                _sinCache[angle] = sinValue;
            }
            return sinValue;
        }

        public static float GetCos(float angle)
        {
            if (!_cosCache.TryGetValue(angle, out float cosValue))
            {
                cosValue = MathF.Cos(angle);
                _cosCache[angle] = cosValue;
            }
            return cosValue;
        }

        public static float GetAtan2(float y, float x)
        {
            var key = (y, x);
            if (!_atan2Cache.TryGetValue(key, out float atan2Value))
            {
                atan2Value = MathF.Atan2(y, x);
                _atan2Cache[key] = atan2Value;
            }
            return atan2Value;
        }
    }
}