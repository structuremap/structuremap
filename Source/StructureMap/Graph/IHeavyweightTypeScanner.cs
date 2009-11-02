using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public interface IHeavyweightTypeScanner
    {
        void Process(PluginGraph graph, IEnumerable<TypeMap> typeMaps);
    }


    public class TypeMap
    {
        internal TypeMap(Type pluginType, IList<Type> concreteTypes)
        {
            PluginType = pluginType;
            ConcreteTypes = new ReadOnlyCollection<Type>(concreteTypes);
        }

        public IList<Type> ConcreteTypes { get; private set; }
        public Type PluginType { get; private set; }
    }

    internal class TypeMapBuilder : ITypeScanner, IDisposable
    {
        private readonly Cache<Type, List<Type>> _implementations = new Cache<Type, List<Type>>(t => new List<Type>());

        public void Process(Type type, PluginGraph graph)
        {
            if (!type.IsConcrete() || !Constructor.HasConstructors(type)) return;
            var pluginTypes = FindPluginTypes(type);

            foreach (var pluginType in pluginTypes)
            {
                _implementations[pluginType].Add(type);
            }
        }

        private IEnumerable<Type> FindPluginTypes(Type type)
        {
            // lets not worry about abstract base classes for now
            return type.GetInterfaces().Where(t => t.IsPublic);
        }

        public IEnumerable<TypeMap> GetTypeMaps()
        {
            return _implementations.Contents().Select(pair => new TypeMap(pair.Key, pair.Value));
        }

        public void Dispose()
        {
            _implementations.Clear();
        }
    }
}