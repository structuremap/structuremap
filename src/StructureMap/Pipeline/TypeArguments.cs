using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class TypeArguments
    {
        internal IDictionary<Type, object> Defaults { get; } = new Dictionary<Type, object>();

        public TypeArguments Set<T>(T arg)
        {
            return Set(typeof(T), arg);
        }

        public TypeArguments Set(Type pluginType, object arg)
        {
            Defaults.Add(pluginType, arg);

            return this;
        }
    }
}