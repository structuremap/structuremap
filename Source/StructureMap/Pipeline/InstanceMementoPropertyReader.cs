using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class InstanceMementoPropertyReader : IArgumentVisitor
    {
        private readonly ConfiguredInstance _instance;
        private readonly InstanceMemento _memento;
        private readonly PluginGraph _pluginGraph;
        private readonly Type _pluginType;

        public InstanceMementoPropertyReader(ConfiguredInstance instance, InstanceMemento memento,
                                             PluginGraph pluginGraph, Type pluginType)
        {
            _instance = instance;
            _memento = memento;
            _pluginGraph = pluginGraph;
            _pluginType = pluginType;
        }

        #region IArgumentVisitor Members

        public void PrimitiveSetter(PropertyInfo property)
        {
            copyPrimitive(property.Name);
        }

        public void StringSetter(PropertyInfo property)
        {
            copyPrimitive(property.Name);
        }

        public void EnumSetter(PropertyInfo property)
        {
            copyPrimitive(property.Name);
        }

        public void ChildSetter(PropertyInfo property)
        {
            copyChild(property.Name, property.PropertyType);
        }

        public void ChildArraySetter(PropertyInfo property)
        {
            copyChildArray(property.Name, property.PropertyType.GetElementType());
        }

        public void PrimitiveParameter(ParameterInfo parameter)
        {
            copyPrimitive(parameter.Name);
        }

        public void StringParameter(ParameterInfo parameter)
        {
            copyPrimitive(parameter.Name);
        }

        public void EnumParameter(ParameterInfo parameter)
        {
            copyPrimitive(parameter.Name);
        }

        public void ChildParameter(ParameterInfo parameter)
        {
            copyChild(parameter.Name, parameter.ParameterType);
        }

        public void ChildArrayParameter(ParameterInfo parameter)
        {
            copyChildArray(parameter.Name, parameter.ParameterType.GetElementType());
        }

        #endregion

        private void copyPrimitive(string name)
        {
            _instance.SetProperty(name, _memento.GetProperty(name));
        }

        private void copyChild(string name, Type childType)
        {
            InstanceMemento child = _memento.GetChildMemento(name);

            Instance childInstance = child == null ? new DefaultInstance() : child.ReadInstance(_pluginGraph, childType);
            _instance.SetChild(name, childInstance);
        }

        private void copyChildArray(string name, Type childType)
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