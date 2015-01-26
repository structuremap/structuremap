using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    internal static class BasicExtensions
    {
        public static string ToName(this ILifecycle lifecycle)
        {
            return lifecycle == null ? Lifecycles.Transient.Description : lifecycle.Description;
        }

        public static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value)) return;
            list.Add(value);
        }

        public static void SafeDispose(this object target)
        {
            var disposable = target as IDisposable;
            if (disposable == null) return;

            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public static void TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Action<TValue> action)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                action(value);
            }
        }

        internal static T As<T>(this object target) where T : class
        {
            return target as T;
        }

        public static bool IsIn<T>(this T target, IList<T> list)
        {
            return list.Contains(target);
        }
    }
}