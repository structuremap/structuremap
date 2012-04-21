using System;
using System.Collections.Generic;

namespace StructureMap.Util
{
    public class TypeDictionary<TValue> : Dictionary<Type, TValue>
    {
        public new bool ContainsKey(Type key)
        {
            TValue value;
            return TryGetValue(key, out value);
        }

        public new TValue this[Type type]
        {
            get
            {
                TValue value;
                if (TryGetValue(type, out value))
                    return value;

                throw new ArgumentOutOfRangeException("type", type, "Was not found");
            }
            set { base[type] = value; }
        }

        public new bool TryGetValue(Type key, out TValue value)
        {
            if (base.TryGetValue(key, out value))
                return true;

            if (key.IsGenericType)
            {
                var genericDefinition = key.GetGenericTypeDefinition();
                if (genericDefinition == null) throw new InvalidOperationException();
                return base.TryGetValue(genericDefinition, out value);
            }
            return false;
        }
    }
}
