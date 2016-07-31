using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class ExplicitArguments
    {
        private readonly IDictionary<string, object> _args;

        public ExplicitArguments(IDictionary<string, object> args)
        {
            _args = args;
        }

        public ExplicitArguments()
            : this(new Dictionary<string, object>())
        {
        }

        public IDictionary<Type, object> Defaults { get; } = new Dictionary<Type, object>();

        public T Get<T>() where T : class
        {
            return (T) Get(typeof (T));
        }

        public object Get(Type type)
        {
            return Defaults.ContainsKey(type) ? Defaults[type] : null;
        }

        public ExplicitArguments Set<T>(T arg)
        {
            return Set(typeof (T), arg);
        }

        public ExplicitArguments Set(Type pluginType, object arg)
        {
            Defaults.Add(pluginType, arg);

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

            Defaults.Each(pair => instance.Dependencies.Add(pair.Key, pair.Value));
        }

        public bool Has(Type type)
        {
            return Defaults.ContainsKey(type);
        }

        public bool Has(string propertyName)
        {
            return _args.ContainsKey(propertyName);
        }

        public override string ToString()
        {
            var values =
                _args.Select(x => "{0}={1}".ToFormat(x.Key, x.Value))
                    .Union(Defaults.Select(x => "{0}={1}".ToFormat(x.Key, x.Value)))
                    .ToArray();

            return "{" + string.Join("; ", values) + "}";
        }

        public bool OnlyNeedsDefaults()
        {
            if (Defaults.Keys.Any(x => x.IsSimple())) return false;
            return !_args.Any();
        }
    }
}