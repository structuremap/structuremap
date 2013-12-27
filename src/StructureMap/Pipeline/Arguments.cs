using System;
using StructureMap.Construction;

namespace StructureMap.Pipeline
{
    [Obsolete("Going to remove in favor of the new 'BuildPlan'")]
    public class Arguments : IArguments
    {
        private readonly IConfiguredInstance _instance;
        private readonly BuildSession _session;

        public Arguments(IConfiguredInstance instance, BuildSession session)
        {
            _instance = instance;
            _session = session;
        }

        public T Get<T>(string propertyName)
        {
            return _instance.Get<T>(propertyName, _session);
        }

        public bool Has(string propertyName)
        {
            return _instance.HasProperty(propertyName, _session);
        }
    }
}