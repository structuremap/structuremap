using System;
using Rhino.Mocks;

namespace StructureMap.AutoMocking
{
    public class RhinoMocksServiceLocator : ServiceLocator
    {
        private readonly MockRepository _mocks;
        private readonly Func<MockRepository, Type, object> mockCreationStrategy;

        public RhinoMocksServiceLocator(MockRepository mocks, Func<MockRepository, Type, object> mockCreationStrategy)
        {
            _mocks = mocks;
            this.mockCreationStrategy = mockCreationStrategy;
        }

        public RhinoMocksServiceLocator(MockRepository mocks) : this(mocks, MockCreationStrategy.RecordMode)
        {
        }

        public RhinoMocksServiceLocator() : this(new MockRepository())
        {
        }

        #region ServiceLocator Members

        public T Service<T>()
        {
            return (T)mockCreationStrategy(_mocks, typeof (T));
        }

        public object Service(Type serviceType)
        {
            return mockCreationStrategy(_mocks, serviceType);
        }

        #endregion
    }

    public static class MockCreationStrategy
    {
        public static object RecordMode(MockRepository repository, Type type)
        {
            return repository.DynamicMock(type);
        }

        public static object ReplayMode(MockRepository repository, Type type)
        {
            var mock = repository.DynamicMock(type);
            repository.Replay(mock);
            return mock;
        }
    }

}