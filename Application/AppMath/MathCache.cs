namespace Application.AppMath
{
    public static class MathCache
    {
        private static readonly Dictionary<int, float> _degreeRadianPairs        = [];
        private static readonly Dictionary<float, float> _sinCache               = [];
        private static readonly Dictionary<float, float> _cosCache               = [];
        private static readonly Dictionary<(float, float), float> _atan2Cache    = [];

        private static readonly OffsetCache _offsetCache = new();
        
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

        public static void CacheOffset(float offsetX, float offsetY) {
            _offsetCache.AddToCache(offsetX, offsetY);
        }
    }
}