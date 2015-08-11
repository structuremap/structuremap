using System;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class Argument
    {
        public string Name;
        public Type Type;
        public object Dependency;

        public Argument CloseType(params Type[] types)
        {
            var clone = new Argument
            {
                Name = Name
            };

            if (Type != null)
            {
                clone.Type = Type.IsOpenGeneric() ? Type.MakeGenericType(types) : Type;
            }

            if (Dependency is Instance)
            {
                clone.Dependency = Dependency.As<Instance>().CloseType(types) ?? Dependency;
            }
            else
            {
                clone.Dependency = Dependency;
            }

            return clone;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}, Dependency: {2}", Name, Type, Dependency);
        }
    }
}