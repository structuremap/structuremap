using System;
using System.Collections.Generic;
using System.ComponentModel;
using StructureMap.Graph;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public class ConstructorInstance : Instance
    {
        private readonly Cache<string, Instance> _dependencies = new Cache<string, Instance>();
        private readonly Plugin _plugin;

        public ConstructorInstance(Type pluggedType)
        {
            _plugin = PluginCache.GetPlugin(pluggedType);
        }

        protected override string getDescription()
        {
            return "Configured Instance of " + _plugin.PluggedType.AssemblyQualifiedName;
        }

        public void SetChild(string name, Instance instance)
        {
            _dependencies[name] = instance;
        }

        public void SetValue(string name, object value)
        {
            var dependencyType = _plugin.FindArgumentType(name);
            var instance = buildInstanceForType(dependencyType, value);
            _dependencies[name] = instance;
        }

        public void SetCollection(string name, IEnumerable<Instance> children)
        {
            var dependencyType = _plugin.FindArgumentType(name);
            var instance = new EnumerableInstance(dependencyType, children);
            _dependencies[name] = instance;
        }

        private static Instance buildInstanceForType(Type dependencyType, object value)
        {
            if (value == null) return new NullInstance();


            if (dependencyType.IsSimple() || dependencyType.IsNullable() || dependencyType == typeof(Guid) || dependencyType == typeof(DateTime))
            {
                var converter = TypeDescriptor.GetConverter(dependencyType);
                var convertedValue = converter.ConvertFrom(value);
                return new ObjectInstance(convertedValue);
            }


            return new ObjectInstance(value);
        }

        public object Get(string propertyName, Type pluginType, BuildSession session)
        {
            return _dependencies[propertyName].Build(pluginType, session);
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            throw new NotImplementedException();
        }

        public object Build(Type pluginType, BuildSession session, InstanceBuilder builder)
        {
            throw new NotImplementedException();   
        }

        public static ConstructorInstance For<T>()
        {
            return new ConstructorInstance(typeof(T));
        }
    }
}