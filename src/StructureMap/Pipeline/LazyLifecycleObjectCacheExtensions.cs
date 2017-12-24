using System;
using System.Collections.Concurrent;

namespace StructureMap.Pipeline
{
    public static class LazyLifecycleObjectCacheExtensions
    {
        public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, LazyLifecycleObject<TValue>> cache, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            var lazy = cache.AddOrUpdate(key, k => new LazyLifecycleObject<TValue>(() => addValue), (k, currentValue) => new LazyLifecycleObject<TValue>(() => updateValueFactory(k, currentValue.Value)));
            return lazy.Value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, LazyLifecycleObject<TValue>> cache, TKey key, Func<TKey, TValue> valueFactory)
        {
            var lazy = cache.GetOrAdd(key, k => new LazyLifecycleObject<TValue>(() => valueFactory(k)));
            return lazy.Value;
        }

        public static bool TryGetValue<TKey, TValue>(this ConcurrentDictionary<TKey, LazyLifecycleObject<TValue>> cache, TKey key, out TValue value)
        {
            LazyLifecycleObject<TValue> lazy;
            var result = cache.TryGetValue(key, out lazy);

            value = result ? lazy.Value : default(TValue);

            return result;
        }

        public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, LazyLifecycleObject<TValue>> cache, TKey key, out TValue value)
        {
            LazyLifecycleObject<TValue> lazy;
            var result = cache.TryRemove(key, out lazy);

            value = result ? lazy.Value : default(TValue);

            return result;
        }
    }
}