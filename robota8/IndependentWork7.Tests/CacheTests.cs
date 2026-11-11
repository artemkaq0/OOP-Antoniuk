using System;
using System.Collections.Generic;
using Xunit;
using IndependentWork7;

namespace IndependentWork7.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_ShouldIncreaseCount_WhenKeyIsNew()
        {
            var cache = new Cache<int, string>(5);
            cache.Add(1, "Apple");
            
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        public void Get_ShouldReturnCorrectValue_WhenKeyExists()
        {
            var cache = new Cache<string, string>(5);
            cache.Add("UA", "Ukraine");
            
            var result = cache.Get("UA");
            
            Assert.Equal("Ukraine", result);
        }

        [Fact]
        public void Add_ShouldEvictOldestElement_WhenCapacityExceeded()
        {
            var cache = new Cache<int, string>(2);
            
            cache.Add(1, "First");
            cache.Add(2, "Second");
            cache.Add(3, "Third");

            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.True(cache.ContainsKey(3));
            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void Get_ShouldThrowKeyNotFoundException_WhenKeyIsMissing()
        {
            var cache = new Cache<int, string>(5);
            
            var exception = Assert.Throws<KeyNotFoundException>(() => cache.Get(99));
            Assert.Contains("99", exception.Message);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueForExistingAndFalseForMissing()
        {
            var cache = new Cache<int, double>(3);
            cache.Add(1, 10.5);

            Assert.True(cache.ContainsKey(1));
            Assert.False(cache.ContainsKey(2));
        }

        [Fact]
        public void Count_ShouldNotExceedCapacity()
        {
            var cache = new Cache<int, int>(3);
            
            for (int i = 0; i < 10; i++)
            {
                cache.Add(i, i * 10);
            }

            Assert.Equal(3, cache.Count);
        }
    }
}