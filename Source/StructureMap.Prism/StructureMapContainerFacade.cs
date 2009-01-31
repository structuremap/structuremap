using System;
using Microsoft.Practices.Composite;

namespace StructureMap.Prism
{
    public class StructureMapContainerFacade : IContainerFacade
    {
        private readonly IContainer _container;

        public StructureMapContainerFacade(IContainer container)
        {
            _container = container;
        }

        public object Resolve(Type type)
        {
            return _container.GetInstance(type);
        }

        public object TryResolve(Type type)
        {
            return _container.TryGetInstance(type);
        }

        public IContainer Container
        {
            get { return _container; }
        }
    }
}