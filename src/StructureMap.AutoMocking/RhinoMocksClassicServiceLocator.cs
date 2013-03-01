using System;

namespace StructureMap.AutoMocking
{
    public class RhinoMocksClassicServiceLocator : ServiceLocator
    {
        private readonly RhinoMockRepositoryProxy _mocks = new RhinoMockRepositoryProxy();

        public T Service<T>() where T : class
        {
            return (T) _mocks.DynamicMock(typeof (T));
        }

        public object Service(Type serviceType)
        {
            return _mocks.DynamicMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return (T) _mocks.PartialMock(typeof (T), args);
        }
    }
}