using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting.Parameters
{
	/// <summary>
	/// Implementation of ParameterEmitter for a constructor argument like
	/// MyClass(IStrategy[] strategies)
	/// </summary>
	public class ChildArrayParameterEmitter : ParameterEmitter
	{
		protected override bool canProcess(Type parameterType)
		{
			bool returnValue = false;

			if (parameterType.IsArray)
			{
				returnValue = (!parameterType.GetElementType().IsPrimitive);
			}

			return returnValue;
		}

		protected override void generate(ILGenerator ilgen, ParameterInfo parameter)
		{
			Type parameterType = parameter.ParameterType;
			string parameterName = parameter.Name;


			putChildArrayFromInstanceMementoOntoStack(ilgen, parameterType, parameterName);
		}

		private void putChildArrayFromInstanceMementoOntoStack(ILGenerator ilgen, Type argumentType, string argumentName)
		{
			ilgen.Emit(OpCodes.Ldarg_0);
			this.callGetInstanceManager(ilgen);
			ilgen.Emit(OpCodes.Ldstr, argumentType.GetElementType().FullName);
			ilgen.Emit(OpCodes.Ldarg_1);

			ilgen.Emit(OpCodes.Ldstr, argumentName);
			this.callInstanceMemento(ilgen, "GetChildrenArray");

			MethodInfo methodCreateInstanceArray = (typeof (InstanceManager).GetMethod("CreateInstanceArray"));
			ilgen.Emit(OpCodes.Callvirt, methodCreateInstanceArray);
			this.cast(ilgen, argumentType);
		}

		protected override void generateSetter(ILGenerator ilgen, PropertyInfo property)
		{
			ilgen.Emit(OpCodes.Ldloc_0);
			putChildArrayFromInstanceMementoOntoStack(ilgen, property.PropertyType, property.Name);

			MethodInfo method = property.GetSetMethod();
			ilgen.Emit(OpCodes.Callvirt, method);
		}


	}
}