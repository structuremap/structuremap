using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class ReferenceMementoBuilder : IMementoBuilder
    {
        private InstanceMemento _memento;

        public ReferenceMementoBuilder(string referenceKey)
        {
            _memento = MemoryInstanceMemento.CreateReferencedInstanceMemento(referenceKey);
        }

        InstanceMemento IMementoBuilder.BuildMemento(PluginFamily family)
        {
            return _memento;
        }

        InstanceMemento IMementoBuilder.BuildMemento(PluginGraph graph)
        {
            return _memento;
        }

        void IMementoBuilder.SetInstanceName(string instanceKey)
        {
        }

        void IMementoBuilder.ValidatePluggability(Type pluginType)
        {
        }


        void IExpression.Configure(PluginGraph graph)
        {
            // no-op;
        }
    }
}