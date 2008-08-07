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
        public void Ctor(ILGenerator ilgen, ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            string parameterName = parameter.Name;

            putEnumerationValueFromMementoOntoStack(ilgen, parameterType, parameterName);
        }

        private void putEnumerationValueFromMementoOntoStack(ILGenerator ilgen, Type argumentType, string argumentName)
        {
            ilgen.Emit(OpCodes.Ldtoken, argumentType);
            ilgen.Emit(OpCodes.Call, Methods.GET_TYPE_FROM_HANDLE);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, argumentName);
            ilgen.Emit(OpCodes.Callvirt, Methods.GET_PROPERTY);
            ilgen.Emit(OpCodes.Ldc_I4_1);

            ilgen.Emit(OpCodes.Call, Methods.ENUM_PARSE);

            ilgen.Emit(OpCodes.Unbox, argumentType);
            ilgen.Emit(OpCodes.Ldind_I4);
        }

        public override void MandatorySetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            putEnumerationValueFromMementoOntoStack(ilgen, property.PropertyType, property.Name);

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}