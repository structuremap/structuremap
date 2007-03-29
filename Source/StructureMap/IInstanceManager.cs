using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap
{
    public interface IInstanceManager
    {
        T CreateInstance<T>(string instanceKey);
        T CreateInstance<T>();
        T FillDependencies<T>();
        void Inject<T>(T instance);
        IList<T> GetAllInstances<T>();
        void SetDefaultsToProfile(string profile);

        InstanceDefaultManager DefaultManager { get; }
    }
}