using System;

namespace StructureMap.AutoMocking.Moq
{
    public class MoqServiceLocator : ServiceLocator
    {
        private readonly MoqFactory _moqs = new MoqFactory();

        public T Service<T>() where T : class
        {
            return (T) _moqs.CreateMock(typeof (T));
        }

        public object Service(Type serviceType)
        {
            return _moqs.CreateMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return (T) _moqs.CreateMockThatCallsBase(typeof (T), args);
        }
    }
}