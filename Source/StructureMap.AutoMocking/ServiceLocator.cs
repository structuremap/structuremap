using System;
using Rhino.Mocks;

namespace StructureMap.AutoMocking
{
    public interface ServiceLocator
    {
        T Service<T>() where T : class;
        object Service(Type serviceType);
        T PartialMock<T>(params object[] args) where T : class;
    }

    public class RhinoMocksClassicServiceLocator : ServiceLocator
    {
        private readonly MockRepository _mocks;

        public RhinoMocksClassicServiceLocator(MockRepository repository)
        {
            _mocks = repository;
        }

        public RhinoMocksClassicServiceLocator() : this(new MockRepository())
        {
        }

        public T Service<T>() where T : class
        {
            return _mocks.DynamicMock<T>();
        }

        public object Service(Type serviceType)
        {
            return _mocks.DynamicMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return _mocks.PartialMock<T>(args);
        }
    }

    public class RhinoMocksAAAServiceLocator : ServiceLocator
    {
        public T Service<T>() where T : class
        {
            return MockRepository.GenerateMock<T>();
        }

        public object Service(Type serviceType)
        {
            var mock = new MockRepository().DynamicMock(serviceType);
            mock.Replay();

            return mock;
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            T mock = new MockRepository().PartialMock<T>(args);
            mock.Replay();

            return mock;
        }
    }
}