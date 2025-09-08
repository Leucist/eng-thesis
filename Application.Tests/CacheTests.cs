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

        [Fact]
        public void MathCache_GetRadianValue_ShouldCacheValue()
        {
            // Arrange
            int degree = 90;
            float expectedRadian = degree.ToRadians();

            // Act
            float firstCall = MathCache.GetRadianValue(degree);
            float secondCall = MathCache.GetRadianValue(degree);

            // Assert
            Assert.Equal(expectedRadian, firstCall);
            Assert.Equal(firstCall, secondCall);
        }

        [Fact]
        public void MathCache_GetSinAndCos_ShouldCacheValues()
        {
            // Arrange
            float angle = MathF.PI / 4; // 45 degrees

            // Act
            float sinFirstCall  = MathCache.GetSin(angle);
            float sinSecondCall = MathCache.GetSin(angle);

            float cosFirstCall  = MathCache.GetCos(angle);
            float cosSecondCall = MathCache.GetCos(angle);

            // Assert
            Assert.Equal(sinFirstCall, sinSecondCall);  // Ensure the same value is returned
            Assert.Equal(cosFirstCall, cosSecondCall);  // Ensure the same value is returned
        }



        // * - - - MATH TESTS - - - * //

        [Fact]
        public void MathCache_GetRadianValue_ShouldReturnCorrectRadian()
        {
            // Arrange
            int degree = 180;
            float expectedRadian = MathF.PI;

            // Act
            float result = MathCache.GetRadianValue(degree);

            // Assert
            Assert.Equal(expectedRadian, result);
        }

        [Fact]
        public void MathCache_GetSin_ShouldReturnCorrectValue()
        {
            // Arrange
            float angle = MathF.PI / 2; // 90 degrees
            float expectedSin = 1.0f;

            // Act
            float result = MathCache.GetSin(angle);

            // Assert
            Assert.Equal(expectedSin, result);
        }

        [Fact]
        public void MathCache_GetCos_ShouldReturnCorrectValue()
        {
            // Arrange
            float angle = 0;
            float expectedCos = 1.0f;

            // Act
            float result = MathCache.GetCos(angle);

            // Assert
            Assert.Equal(expectedCos, result);
        }

        [Fact]
        public void MathCache_GetAtan2_ShouldReturnCorrectValue()
        {
            // Arrange
            float y = 1;
            float x = 1;
            float expectedAtan2 = MathF.Atan2(y, x); // Should be pi/4

            // Act
            float result = MathCache.GetAtan2(y, x);

            // Assert
            Assert.Equal(expectedAtan2, result);
        }
    }
}