using System;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class Argument
    {
        /// <summary>
        /// Parameter or property name that matches this Argument. May be null to match on Type only
        /// </summary>
        public string Name;

        /// <summary>
        /// The dependency type of this Argument
        /// </summary>
        public Type Type;

        /// <summary>
        /// The actual dependency value of the "Type" or an Instance object
        /// </summary>
        public object Dependency;

        /// <summary>
        /// Creates a "closed" Argument for open generic dependency types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
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


        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Name: {0}, Type: {1}, Dependency: {2}", Name, Type, Dependency);
        }
    }
}