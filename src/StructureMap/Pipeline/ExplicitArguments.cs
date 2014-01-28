using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Pipeline
{
    public class ExplicitArguments
    {
        private readonly IDictionary<string, object> _args;
        private readonly IDictionary<Type, object> _defaults = new Dictionary<Type, object>();

        public ExplicitArguments(IDictionary<string, object> args)
        {
            _args = args;
        }

        public ExplicitArguments()
            : this(new Dictionary<string, object>())
        {
        }

        public IDictionary<Type, object> Defaults
        {
            get { return _defaults; }
        }

        public T Get<T>() where T : class
        {
            return (T) Get(typeof (T));
        }

        public object Get(Type type)
        {
            return _defaults.ContainsKey(type) ? _defaults[type] : null;
        }

        public ExplicitArguments Set<T>(T arg)
        {
            return Set(typeof (T), arg);
        }

        public ExplicitArguments Set(Type pluginType, object arg)
        {
            _defaults.Add(pluginType, arg);

            return this;
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
            _args.Each(pair => instance.Dependencies.Add(pair.Key, pair.Value));

            _defaults.Each(pair => instance.Dependencies.Add(pair.Key, pair.Value));
        }

        public bool Has(Type type)
        {
            return _defaults.ContainsKey(type);
        }

        public bool Has(string propertyName)
        {
            return _args.ContainsKey(propertyName);
        }

        public override string ToString()
        {
            var values =
                _args.Select(x => "{0}={1}".ToFormat(x.Key, x.Value))
                    .Union(_defaults.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)))
                    .ToArray();

            return "{" + string.Join("; ", values) + "}";

        }
    }
}