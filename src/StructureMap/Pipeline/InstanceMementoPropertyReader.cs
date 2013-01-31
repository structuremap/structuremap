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

        public InstanceMementoPropertyReader(ConfiguredInstance instance, InstanceMemento memento, PluginGraph pluginGraph)
        {
            _instance = instance;
            _memento = memento;
            _pluginGraph = pluginGraph;
        }

        #region IArgumentVisitor Members

        public void PrimitiveSetter(PropertyInfo property, bool isMandatory)
        {
            copyPrimitive(property.Name, isMandatory);
        }

        public void StringSetter(PropertyInfo property, bool isMandatory)
        {
            copyPrimitive(property.Name, isMandatory);
        }

        public void EnumSetter(PropertyInfo property, bool isMandatory)
        {
            copyPrimitive(property.Name, isMandatory);
        }

        public void ChildSetter(PropertyInfo property, bool isMandatory)
        {
            copyChild(property.Name, property.PropertyType);
        }

        public void ChildArraySetter(PropertyInfo property, bool isMandatory)
        {
            copyChildArray(property.Name, property.PropertyType.GetElementType());
        }

        public void PrimitiveParameter(ParameterInfo parameter)
        {
            copyPrimitive(parameter.Name, true);
        }

        public void StringParameter(ParameterInfo parameter)
        {
            copyPrimitive(parameter.Name, true);
        }

        public void EnumParameter(ParameterInfo parameter)
        {
            copyPrimitive(parameter.Name, true);
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

        private void copyPrimitive(string name, bool isMandatory)
        {
            string propertyValue = _memento.GetProperty(name);

            if (propertyValue == null && isMandatory)
            {
                throw new StructureMapException(205, name, _memento.InstanceKey);
            }

            _instance.SetValue(name, propertyValue);
        }

        private void copyChild(string name, Type childType)
        {
            Instance childInstance = _memento.ReadChildInstance(name, _pluginGraph, childType);
            if (childInstance == null) return;

            _instance.SetChild(name, childInstance);
        }

        private void copyChildArray(string name, Type childType)
        {
            InstanceMemento[] mementoes = _memento.GetChildrenArray(name) ?? new InstanceMemento[0];

            var children = new Instance[mementoes.Length];
            for (int i = 0; i < mementoes.Length; i++)
            {
                InstanceMemento memento = mementoes[i];
                children[i] = memento.ReadInstance(_pluginGraph, childType);
            }

            _instance.SetCollection(name, children);
        }
    }
}