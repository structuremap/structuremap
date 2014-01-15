using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class Policies
    {
        public readonly SetterRules SetterRules = new SetterRules();
        public readonly ConstructorSelector ConstructorSelector = new ConstructorSelector();

        public bool CanBeAutoFilled(Type concreteType)
        {
            var ctor = SelectConstructor(concreteType);

            if (ctor == null) return false;

            foreach (var parameter in ctor.GetParameters())
            {
                if (!parameter.ParameterType.IsAutoFillable())
                {
                    return false;
                }
            }

            var mandatories = ConcreteType.GetSetters(concreteType).Where(IsMandatorySetter).ToArray();
            if (mandatories.Any())
            {
                return mandatories.All(x => x.PropertyType.IsAutoFillable());
            }

            return true;
        }

        public bool IsMandatorySetter(PropertyInfo propertyInfo)
        {
            return SetterRules.IsMandatory(propertyInfo);
        }

        public ConstructorInfo SelectConstructor(Type pluggedType)
        {
            return ConstructorSelector.Select(pluggedType);
        }
    }
}