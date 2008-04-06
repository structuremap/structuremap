using System;

namespace StructureMap.Pipeline
{
    public interface IConfiguredInstance
    {
        InstanceMemento[] GetChildrenArray(string propertyName);
        string GetProperty(string propertyName);
        object GetChild(string propertyName, string typeName, IInstanceCreator instanceCreator);
    }

    public class ConfiguredInstance : Instance
    {
        protected override T build<T>(IInstanceCreator creator)
        {
            throw new NotImplementedException();
        }

        public override void Diagnose<T>(IInstanceCreator creator, IInstanceDiagnostics diagnostics)
        {
            throw new NotImplementedException();
        }

        public override void Describe<T>(IInstanceDiagnostics diagnostics)
        {
            throw new NotImplementedException();
        }
    }
}