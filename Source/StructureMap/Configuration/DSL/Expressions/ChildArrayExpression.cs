using System;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL.Expressions
{
    public class ChildArrayExpression<PLUGINTYPE> : IExpression
    {
        private readonly MemoryInstanceMemento _memento;
        private readonly InstanceExpression _parent;
        private readonly string _propertyName;
        private IMementoBuilder[] _builders;
        private Type _pluginType = typeof (PLUGINTYPE);

        public ChildArrayExpression(InstanceExpression parent, MemoryInstanceMemento memento, string propertyName)
        {
            _parent = parent;
            _memento = memento;
            _propertyName = propertyName;

            _pluginType = typeof (PLUGINTYPE).GetElementType();
        }

        #region IExpression Members

        void IExpression.Configure(PluginGraph graph)
        {
            PluginFamily family = graph.LocateOrCreateFamilyForType(_pluginType);
            InstanceMemento[] childMementos = new InstanceMemento[_builders.Length];
            for (int i = 0; i < _builders.Length; i++)
            {
                InstanceMemento memento = processMementoBuilder(_builders[i], family, graph);
                childMementos[i] = memento;
            }

            _memento.AddChildArray(_propertyName, childMementos);
        }

        #endregion

        private InstanceMemento processMementoBuilder(IMementoBuilder builder, PluginFamily family, PluginGraph graph)
        {
            builder.ValidatePluggability(_pluginType);
            builder.Configure(graph);
            return builder.BuildMemento(family);
        }

        public InstanceExpression Contains(params IMementoBuilder[] builders)
        {
            _builders = builders;

            return _parent;
        }
    }
}