using System;

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
        private readonly RhinoMockRepositoryProxy _mocks = new RhinoMockRepositoryProxy();

        public T Service<T>() where T : class
        {
            return (T)_mocks.DynamicMock(typeof(T));
        }

        public object Service(Type serviceType)
        {
            return _mocks.DynamicMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return (T)_mocks.PartialMock(typeof(T), args);
        }
    }

    public class RhinoMocksAAAServiceLocator : ServiceLocator
    {
        private readonly RhinoMockRepositoryProxy _mocks = new RhinoMockRepositoryProxy();

        public T Service<T>() where T : class
        {
            var instance = (T)_mocks.DynamicMock(typeof (T));
            _mocks.Replay(instance);
            return instance;
        }

        public object Service(Type serviceType)
        {
            var instance = _mocks.DynamicMock(serviceType);
            _mocks.Replay(instance);
            return instance;
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            var instance = (T)_mocks.PartialMock(typeof(T), args);
            _mocks.Replay(instance);
            return instance;
        }
    }

    public class MoqServiceLocator : ServiceLocator
    {
        private readonly MoqFactory _moqs = new MoqFactory();
        
        public T Service<T>() where T : class
        {
            return (T)_moqs.CreateMock(typeof(T));
        }

        public object Service(Type serviceType)
        {
            return _moqs.CreateMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return (T)_moqs.CreateMockThatCallsBase(typeof (T), args);
        }
    }
}