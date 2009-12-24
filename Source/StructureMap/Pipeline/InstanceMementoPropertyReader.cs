using System;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public class InstanceMementoPropertyReader : IArgumentVisitor
    {
        private readonly IConfiguredInstance _instance;
        private readonly InstanceMemento _memento;
        private readonly PluginGraph _pluginGraph;
        private readonly Type _pluginType;

        public InstanceMementoPropertyReader(IConfiguredInstance instance, InstanceMemento memento,
                                             PluginGraph pluginGraph, Type pluginType)
        {
            _instance = instance;
            _memento = memento;
            _pluginGraph = pluginGraph;
            _pluginType = pluginType;
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
            copyChild(property.Name, property.PropertyType, isMandatory);
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
            copyChild(parameter.Name, parameter.ParameterType, true);
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

            _instance.SetProperty(name, propertyValue);
        }

        private void copyChild(string name, Type childType, bool isMandatory)
        {
            Instance childInstance = _memento.ReadChildInstance(name, _pluginGraph, childType);

            _instance.Set(name, childInstance);
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

            _instance.SetChildArray(name, childType, children);
        }
    }
}