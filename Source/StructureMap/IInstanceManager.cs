using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap
{
    public interface IInstanceManager
    {
        InstanceDefaultManager DefaultManager { get; }
        T CreateInstance<T>(string instanceKey);
        T CreateInstance<T>();
        T FillDependencies<T>();
        void InjectStub<T>(T instance);
        IList<T> GetAllInstances<T>();
        void SetDefaultsToProfile(string profile);

        T CreateInstance<T>(InstanceMemento memento);
    }
}