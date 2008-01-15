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
            return _mocks.CreateMock<T>();
        }

        public object Service(Type serviceType)
        {
            return _mocks.CreateMock(serviceType);
        }

        #endregion
    }
}