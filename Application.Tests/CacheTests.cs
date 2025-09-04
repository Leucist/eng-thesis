using Application.AppMath;

namespace Application.Tests
{
    public class CacheTests
    {
        [Fact]
        public void LRUCache_StoredValue_ShouldBeAccessible() {
            int cahceCapacity = 5;
            var cache = new LRUCache<float, float>(cahceCapacity);

            int storedValue0 = 8;
            int storedValue1 = 9;
            int storedValue2 = 8;
            int storedValue3 = 10;
            int storedValue4 = 13;

            // Fill the cache
            cache.Add(0, storedValue0);
            cache.Add(1, storedValue1);
            cache.Add(2, storedValue2);
            cache.Add(3, storedValue3);
            cache.Add(4, storedValue4);

            // Test that the value is properly stored and can be accessed
            Assert.Equal(storedValue0, cache.Get(0));
            Assert.Equal(storedValue1, cache.Get(1));
            Assert.Equal(storedValue2, cache.Get(2));
            Assert.Equal(storedValue3, cache.Get(3));
            Assert.Equal(storedValue4, cache.Get(4));
        }

        [Fact]
        public void LRUCache_AddingBeyondCapacity_ShouldEvictLeastRecentlyUsedItem() {
            int cahceCapacity = 5;
            var cache = new LRUCache<float, float>(cahceCapacity);

            // Initial filling
            cache.Add(0, 0);
            cache.Add(1, 1);
            cache.Add(2, 2);
            cache.Add(3, 3);
            cache.Add(4, 4);
            // Access the value by key "0"
            cache.Get(0);

            // Adding new value beyond capacity, therefore eliminating the LRU entity - (1, 1) - as (0, 0) was recently accessed
            cache.Add(5, 13);
            Assert.Equal(13, cache.Get(5));
            Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
        }
    }
}