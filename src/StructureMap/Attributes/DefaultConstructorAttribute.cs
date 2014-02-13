using System;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap
{
    /// <summary>
    /// Used to override the constructor of a class to be used by StructureMap to create
    /// a Pluggable object
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DefaultConstructorAttribute : Attribute
    {
        /// <summary>
        /// Examines a System.Type object and determines the ConstructorInfo to use in creating
        /// instances of the Type
        /// </summary>
        /// <param name="ExportedType"></param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(Type ExportedType)
        {
            ConstructorInfo returnValue = null;

            foreach (var constructor in ExportedType.GetConstructors())
            {
                var atts = constructor.GetCustomAttributes(typeof (DefaultConstructorAttribute), true);

                if (atts.Any())
                {
                    returnValue = constructor;
                    break;
                }
            }

            return returnValue;
        }
    }
}