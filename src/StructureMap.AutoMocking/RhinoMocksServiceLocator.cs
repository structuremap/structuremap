using System;
using Rhino.Mocks;

namespace StructureMap.AutoMocking
{
    public class RhinoMocksServiceLocator : ServiceLocator
    {
        private readonly MockRepository _mocks;
        private readonly Func<object, object> _mockDecorator;

        public RhinoMocksServiceLocator(MockRepository mocks, Func<object, object> mockDecorator)
        {
            _mocks = mocks;
            _mockDecorator = mockDecorator;
        }

        public RhinoMocksServiceLocator(MockRepository mocks) : this(mocks, MockDecorator.Nullo)
        {
        }

        public RhinoMocksServiceLocator() : this(new MockRepository())
        {
        }

        #region ServiceLocator Members

        public T Service<T>()
        {
            return (T) Service(typeof (T));
        }

        public object Service(Type serviceType)
        {
            return _mockDecorator(_mocks.DynamicMock(serviceType));
        }

        #endregion
    }

    public static class MockDecorator
    {
        public static object Nullo(object mock)
        {
            return mock;
        }

        public static object PutInReplayMode(object mock)
        {
            mock.Replay();
            return mock;
        }
    }

}