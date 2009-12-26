using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class ExplicitArguments
    {
        private readonly IDictionary<string, object> _args;
        private readonly IDictionary<Type, object> _children = new Dictionary<Type, object>();

        public ExplicitArguments(IDictionary<string, object> args)
        {
            _args = args;
        }

        public ExplicitArguments()
            : this(new Dictionary<string, object>())
        {
        }

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
            Set(typeof (T), arg);
        }

        public void Set(Type pluginType, object arg)
        {
            _children.Add(pluginType, arg);
        }

        public void SetArg(string key, object argValue)
        {
            _args.Add(key, argValue);
        }

        public object GetArg(string key)
        {
            return _args.ContainsKey(key) ? _args[key] : null;
        }

        public void Configure(IConfiguredInstance instance)
        {
            _args.Each(pair => { instance.SetValue(pair.Key, pair.Value); });

            _children.Each(pair => { instance.SetValue(pair.Key, pair.Value); });
        }

        public bool Has(Type type)
        {
            return _children.ContainsKey(type);
        }

        public bool Has(string propertyName)
        {
            return _args.ContainsKey(propertyName);
        }

        public void RegisterDefaults(BuildSession session)
        {
            foreach (var pair in _children)
            {
                session.RegisterDefault(pair.Key, pair.Value);
            }
        }
    }
}