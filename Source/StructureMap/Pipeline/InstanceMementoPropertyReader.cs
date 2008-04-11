using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class InstanceMementoPropertyReader : IPluginArgumentVisitor
    {
        private readonly ConfiguredInstance _instance;
        private readonly InstanceMemento _memento;
        private readonly PluginGraph _pluginGraph;
        private readonly Type _pluginType;

        public InstanceMementoPropertyReader(ConfiguredInstance instance, InstanceMemento memento, PluginGraph pluginGraph, Type pluginType)
        {
            _instance = instance;
            _memento = memento;
            _pluginGraph = pluginGraph;
            _pluginType = pluginType;
        }

        public void Primitive(string name)
        {
            _instance.SetProperty(name, _memento.GetProperty(name));
        }

        public void Child(string name, Type childType)
        {
            InstanceMemento child = _memento.GetChildMemento(name);
            
            Instance childInstance = child == null ? new DefaultInstance() : child.ReadInstance(_pluginGraph, childType);
            _instance.SetChild(name, childInstance);
        }

        public void ChildArray(string name, Type childType)
        {
            InstanceMemento[] mementoes = _memento.GetChildrenArray(name);

            // TODO -- want to default to mementoes == null is all
            if (mementoes == null)
            {
                mementoes = new InstanceMemento[0];
            }

            Instance[] children = new Instance[mementoes.Length];
            for (int i = 0; i < mementoes.Length; i++)
            {
                InstanceMemento memento = mementoes[i];
                children[i] = memento.ReadInstance(_pluginGraph, childType);
            }

            _instance.SetChildArray(name, children);
        }
    }
}