using System;

namespace StructureMap.AutoMocking
{
    public class NSubstituteServiceLocator : ServiceLocator
    {
        private readonly SubstituteFactory _substituteFactory = new SubstituteFactory();

        public T Service<T>() where T : class
        {
            return (T)_substituteFactory.CreateMock(typeof(T));
        }

        public object Service(Type serviceType)
        {
            return _substituteFactory.CreateMock(serviceType);
        }

        public T PartialMock<T>(params object[] args) where T : class
        {
            return (T)_substituteFactory.CreateMock(typeof(T));
        }
    }
}