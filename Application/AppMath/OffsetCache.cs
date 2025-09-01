namespace Application.AppMath
{
    public class OffsetCache
    {
        private const int CACHE_SIZE = 10;

        private int _currentIndex = -1;
        private OffsetEntry[] _offsetCache = new OffsetEntry[CACHE_SIZE];

        public void AddToCache(float xOffset, float yOffset) {
            _currentIndex = (_currentIndex + 1) % CACHE_SIZE;   // moves the index
            _offsetCache[_currentIndex] = new OffsetEntry(xOffset, yOffset);
        }

        public OffsetEntry GetLastValue() {
            return _currentIndex > -1 ? _offsetCache[_currentIndex] : throw new ArgumentOutOfRangeException("Cache is empty.");
        }
    }
}