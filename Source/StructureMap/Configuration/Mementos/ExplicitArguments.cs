using System;
using System.Collections.Generic;

namespace StructureMap.Configuration.Mementos
{
    public class ExplicitArguments
    {
        private readonly Dictionary<string, string> _args = new Dictionary<string, string>();
        private readonly Dictionary<Type, object> _children = new Dictionary<Type, object>();

        public T Get<T>() where T : class
        {
            return (T) Get(typeof (T));
        }

        public object Get(Type type)
        {
            return _children.ContainsKey(type) ? _children[type] : null;
        }

        public void Set<T>(T arg)
        {
            _children.Add(typeof (T), arg);
        }

        public void SetArg(string key, object argValue)
        {
            _args.Add(key, argValue.ToString());
        }

        public string GetArg(string key)
        {
            return _args.ContainsKey(key) ? _args[key] : null;
        }
    }
}