namespace Application.AppMath
{
    public static class MathCache
    {
        private const int CACHE_SIZE = 10;

        private static readonly LRUCache<int, float>    _degreeRadianPairs  = new(CACHE_SIZE);
        private static readonly LRUCache<float, float>  _sinCache           = new(CACHE_SIZE);
        private static readonly LRUCache<float, float>  _cosCache           = new(CACHE_SIZE);
        private static readonly LRUCache<(float, float), float> _atan2Cache = new(CACHE_SIZE);

        private static OffsetEntry _offsetCache = new();

        public static int Capacity => CACHE_SIZE;
        
        private static TValue GetCached<TKey, TValue>(
            LRUCache<TKey, TValue> cache, 
            TKey key, 
            Func<TKey, TValue> valueFactory
        ) where TKey : notnull {
            try {
                return cache.Get(key);
            } catch (KeyNotFoundException) {
                var value = valueFactory(key);
                cache.Add(key, value);
                return value;
            }
        }

        public static float GetRadianValue(int degree)  => GetCached(_degreeRadianPairs, degree, d => d.ToRadians());

        public static float GetSin(float angle)         => GetCached(_sinCache, angle, MathF.Sin);

        public static float GetCos(float angle)         => GetCached(_cosCache, angle, MathF.Cos);

        public static float GetAtan2(float y, float x)  => GetCached(_atan2Cache, (y, x), key => MathF.Atan2(key.Item1, key.Item2));

        public static void CacheOffset(float offsetX, float offsetY) {
            _offsetCache.X = offsetX;
            _offsetCache.Y = offsetY;
        }
    }
}