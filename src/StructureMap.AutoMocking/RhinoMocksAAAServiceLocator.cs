using System;

namespace StructureMap.AutoMocking
{
    public class RhinoMocksAAAServiceLocator : ServiceLocator
    {
        private readonly RhinoMockRepositoryProxy _mocks = new RhinoMockRepositoryProxy();

        public T Service<T>() where T : class
        {
            var instance = (T) _mocks.DynamicMock(typeof (T));
            _mocks.Replay(instance);
            return instance;
        }

        public object Service(Type serviceType)
        {
            object instance = _mocks.DynamicMock(serviceType);
            _mocks.Replay(instance);
            return instance;
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            var instance = (T) _mocks.PartialMock(typeof (T), args);
            _mocks.Replay(instance);
            return instance;
        }
    }
}