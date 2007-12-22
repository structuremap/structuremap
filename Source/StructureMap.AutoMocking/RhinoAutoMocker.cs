using System;
using System.Collections.Generic;
using Rhino.Mocks;

namespace StructureMap.AutoMocking
{
    public delegate void GenericVoidMethod<TARGETCLASS>(TARGETCLASS target);
    public delegate void VoidMethod();

    public class RhinoAutoMocker : MockRepository, ServiceLocator
    {
        private Dictionary<Type, object> _services;
        private AutoMockedInstanceManager _manager;

        public RhinoAutoMocker()
        {
            _services = new Dictionary<Type, object>();
            _manager = new AutoMockedInstanceManager(this);
        }

        public TARGETCLASS Create<TARGETCLASS>()
        {
            throw new NotImplementedException();
        }

        public T Service<T>()
        {
            throw new NotImplementedException();
        }

        public T UsePartialMock<T>()
        {
            throw new NotImplementedException();
        }

        public void Inject<T>(T stub)
        {
            throw new NotImplementedException();
        }

        public object Service(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
