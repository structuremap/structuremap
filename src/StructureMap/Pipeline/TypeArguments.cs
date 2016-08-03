using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class TypeArguments
    {
        public IDictionary<Type, object> Defaults { get; } = new Dictionary<Type, object>();

        public TypeArguments Set<T>(T arg)
        {
            return Set(typeof(T), arg);
        }

        public TypeArguments Set(Type pluginType, object arg)
        {
            if (Defaults.ContainsKey(pluginType))
            {
                Defaults[pluginType] = arg;
            }
            else
            {
                Defaults.Add(pluginType, arg);
            }
            

            return this;
        }

        public bool Has(Type type) => Defaults.ContainsKey(type);

        public T Get<T>()
        {
            return (T) Defaults[typeof(T)];
        }
    }
}