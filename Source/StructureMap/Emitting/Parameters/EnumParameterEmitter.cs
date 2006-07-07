using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting.Parameters
{
	/// <summary>
	/// Implementation of ParameterEmitter for enumeration types
	/// </summary>
	public class EnumParameterEmitter : ParameterEmitter
	{
		protected override bool canProcess(Type parameterType)
		{
			return (parameterType.IsEnum);
		}

		protected override void generate(ILGenerator ilgen, ParameterInfo parameter)
		{
			Type parameterType = parameter.ParameterType;
			string parameterName = parameter.Name;

			putEnumerationValueFromMementoOntoStack(ilgen, parameterType, parameterName);
		}

		private void putEnumerationValueFromMementoOntoStack(ILGenerator ilgen, Type argumentType, string argumentName)
		{
			Type typeItself = typeof (Type);
			MethodInfo getTypeFromHandleMethod = typeItself.GetMethod("GetTypeFromHandle");

			ilgen.Emit(OpCodes.Ldtoken, argumentType);
			ilgen.Emit(OpCodes.Call, getTypeFromHandleMethod);
			ilgen.Emit(OpCodes.Ldarg_1);
			ilgen.Emit(OpCodes.Ldstr, argumentName);
			this.callInstanceMemento(ilgen, "GetProperty");
			ilgen.Emit(OpCodes.Ldc_I4_1);


			Type enumType = typeof (Enum);
			MethodInfo parseMethod = enumType.GetMethod("Parse", new Type[] {typeItself, typeof (string), typeof (bool)});
			ilgen.Emit(OpCodes.Call, parseMethod);

			ilgen.Emit(OpCodes.Unbox, argumentType);
			ilgen.Emit(OpCodes.Ldind_I4);
		}

		protected override void generateSetter(ILGenerator ilgen, PropertyInfo property)
		{
			ilgen.Emit(OpCodes.Ldloc_0);
			putEnumerationValueFromMementoOntoStack(ilgen, property.PropertyType, property.Name);

			MethodInfo method = property.GetSetMethod();
			ilgen.Emit(OpCodes.Callvirt, method);
		}
	}
}