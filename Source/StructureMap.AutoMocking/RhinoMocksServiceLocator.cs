using System;
using Rhino.Mocks;

namespace StructureMap.AutoMocking
{
    public class RhinoMocksServiceLocator : ServiceLocator
    {
        private readonly MockRepository _mocks;

        public RhinoMocksServiceLocator(MockRepository mocks)
        {
            _mocks = mocks;
        }


        public RhinoMocksServiceLocator() : this(new MockRepository())
        {
        }

        #region ServiceLocator Members

        public T Service<T>()
        {
            return _mocks.DynamicMock<T>();
        }

        public object Service(Type serviceType)
        {
            return _mocks.DynamicMock(serviceType);
        }

        #endregion
    }

    public class RhinoMocksAAAServiceLocator : ServiceLocator
    {
        private readonly MockRepository _mocks;

        public RhinoMocksAAAServiceLocator(MockRepository mocks)
        {
            _mocks = mocks;
        }


        public RhinoMocksAAAServiceLocator()
            : this(new MockRepository())
        {
        }

        #region ServiceLocator Members

        public T Service<T>()
        {
            return MockRepository.GenerateMock<T>();
        }

        public object Service(Type serviceType)
        {
            var mock = _mocks.DynamicMock(serviceType);
            _mocks.Replay(mock);
            return mock;
        }

        #endregion
    }
}