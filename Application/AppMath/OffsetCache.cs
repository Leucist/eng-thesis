namespace Application.AppMath
{
    public class OffsetCache
    {
        private const int CACHE_SIZE = 10;

        private int _currentIndex = -1;
        private OffsetEntry[] _offsetCache = new OffsetEntry[CACHE_SIZE];

        public void AddToCache(ForceVector forceA, ForceVector forceB, float xOffset, float yOffset) {
            _currentIndex = (_currentIndex + 1) % CACHE_SIZE;   // moves the index
            _offsetCache[_currentIndex] = new OffsetEntry(forceA, forceB, xOffset, yOffset);
        }

        public OffsetEntry GetLastValue() {
            return _offsetCache[_currentIndex];
        }
    }
}