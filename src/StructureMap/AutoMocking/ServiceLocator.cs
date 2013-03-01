using System;

namespace StructureMap.AutoMocking
{
    public interface ServiceLocator
    {
        T Service<T>() where T : class;
        object Service(Type serviceType);
        T PartialMock<T>(params object[] args) where T : class;
    }
}