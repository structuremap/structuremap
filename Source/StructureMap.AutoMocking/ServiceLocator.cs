using System;

namespace StructureMap.AutoMocking
{
    public interface ServiceLocator
    {
        T Service<T>();
        object Service(Type serviceType);
    }
}