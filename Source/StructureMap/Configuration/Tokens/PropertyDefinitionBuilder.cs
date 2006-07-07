using System;
using System.Reflection;

namespace StructureMap.Configuration.Tokens
{
	public class PropertyDefinitionBuilder
	{
		private PropertyDefinitionBuilder()
		{
		}

		public static PropertyDefinition[] CreatePropertyDefinitions(ConstructorInfo constructor)
		{
			ParameterInfo[] parameters = constructor.GetParameters();
			PropertyDefinition[] returnValue = new PropertyDefinition[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameter = parameters[i];
				PropertyDefinition definition = buildDefinition(parameter.Name, parameter.ParameterType);
				definition.DefinitionType = PropertyDefinitionType.Constructor;

				returnValue[i] = definition;
			}

			return returnValue;
		}

		public static PropertyDefinition CreatePropertyDefinition(PropertyInfo property)
		{
			PropertyDefinition definition = buildDefinition(property.Name, property.PropertyType);
			definition.DefinitionType = PropertyDefinitionType.Setter;

			return definition;
		}

		private static PropertyDefinition buildDefinition(string propertyName, Type memberType)
		{
			PropertyDefinition definition = new PropertyDefinition();
			definition.PropertyName = propertyName;
			definition.PropertyType = memberType.FullName;

			bool isPrimitive = memberType.IsPrimitive || memberType.Equals(typeof (string));
			if (isPrimitive)
			{
				definition.ArgumentType = ArgumentType.Primitive;
			}
			else if (memberType.IsEnum)
			{
				definition.ArgumentType = ArgumentType.Enumeration;
				definition.EnumerationValues = Enum.GetNames(memberType);
			}
			else if (memberType.IsArray)
			{
				definition.ArgumentType = ArgumentType.ChildArray;
				definition.PropertyType = memberType.GetElementType().FullName;
			}
			else
			{
				definition.ArgumentType = ArgumentType.Child;
			}

			return definition;
		}
	}
}
