using System;
using System.Collections.Generic;

namespace lab13v1
{
    public static class FunctionalUtils
    {
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static List<T> Filter<T>(List<T> list, Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                if (predicate(item))
                    result.Add(item);
            }
            return result;
        }

        public static TAccumulate Reduce<T, TAccumulate>(List<T> list, TAccumulate seed, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            TAccumulate result = seed;
            foreach (var item in list)
            {
                result = accumulator(result, item);
            }
            return result;
        }

        public static void ForEach<T>(List<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static bool Any<T>(List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                    return true;
            }
            return false;
        }
    }
}