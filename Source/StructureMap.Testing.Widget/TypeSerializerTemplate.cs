using System;

namespace StructureMap.Testing.Widget
{
    /// <summary>
    /// This class ONLY exists in order to discover the IL for a TypeSerializer
    /// </summary>
    public class TypeSerializerTemplate
    {
        public TypeSerializerTemplate()
        {
        }


        public Type PluginType
        {
            get { return typeof (Rule); }
        }

        public string ConcreteTypeKey
        {
            get { return "ComplexRule"; }
        }

        public object BuildInstance(InstanceMemento Memento)
        {
            return new ComplexRule(
                Memento.GetProperty("String"),
                Memento.GetProperty("String2"),
                int.Parse(Memento.GetProperty("Int")),
                long.Parse(Memento.GetProperty("Long")),
                byte.Parse(Memento.GetProperty("Byte")),
                double.Parse(Memento.GetProperty("Double")),
                bool.Parse(Memento.GetProperty("Bool")));
        }

        public object ReturnNull()
        {
            return null;
        }
    }
}