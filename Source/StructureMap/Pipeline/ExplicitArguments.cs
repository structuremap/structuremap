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

        public ExplicitArguments() : this(new Dictionary<string, object>())
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
            _children.Add(typeof (T), arg);
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
            foreach (var arg in _args)
            {
                if (arg.Value == null) continue;

                instance.SetProperty(arg.Key, arg.Value.ToString());
                instance.SetChild(arg.Key, new LiteralInstance(arg.Value));
            }
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

    public class ExplicitInstance : ConfiguredInstance
    {
        private readonly ExplicitArguments _args;

        public ExplicitInstance(Type pluginType, ExplicitArguments args, BasicInstance defaultInstance) : base(null)
        {
            args.Configure(this);
            _args = args;
            mergeIntoThis(defaultInstance);
        }


        protected override object getChild(string propertyName, Type pluginType, BuildSession buildSession)
        {
            if (_args.Has(pluginType))
            {
                return _args.Get(pluginType);
            }

            if (_args.Has(propertyName))
            {
                return _args.GetArg(propertyName);
            }

            return base.getChild(propertyName, pluginType, buildSession);
        }
    }
}