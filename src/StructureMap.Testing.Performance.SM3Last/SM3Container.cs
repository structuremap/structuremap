using System;
using System.Collections.Generic;

namespace StructureMap.Testing.Performance.SM3Last
{
    public class SM3Container
    {
        private readonly Container _container;

        public SM3Container(IEnumerable<Tuple<Type, Type>> transientTypes, IEnumerable<Tuple<Type, Type>> singletonTypes)
        {
            _container = new Container(cfg =>
            {
                foreach (var map in transientTypes)
                {
                    cfg.For(map.Item1).Use(map.Item2);
                }

                foreach (var map in singletonTypes)
                {
                    cfg.For(map.Item1).Use(map.Item2).Singleton();
                }
            });
        }

        public object GetInstance(Type pluginType)
        {
            return _container.GetInstance(pluginType);
        }
    }
}