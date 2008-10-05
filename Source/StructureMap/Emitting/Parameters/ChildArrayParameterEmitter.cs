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
        public void Ctor(ILGenerator ilgen, ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            string parameterName = parameter.Name;


            putChildArrayFromInstanceMementoOntoStack(ilgen, parameterType, parameterName);
        }

        private void putChildArrayFromInstanceMementoOntoStack(ILGenerator ilgen, Type argumentType, string argumentName)
        {
            ilgen.Emit(OpCodes.Ldarg_2);

            ilgen.Emit(OpCodes.Ldtoken, argumentType.GetElementType());
            ilgen.Emit(OpCodes.Call, Methods.GET_TYPE_FROM_HANDLE);

            ilgen.Emit(OpCodes.Ldarg_1);

            ilgen.Emit(OpCodes.Ldstr, argumentName);
            ilgen.Emit(OpCodes.Callvirt, Methods.GET_CHILDREN_ARRAY);
            ilgen.Emit(OpCodes.Callvirt, Methods.CREATE_INSTANCE_ARRAY);
            cast(ilgen, argumentType);
        }

        public override void MandatorySetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            putChildArrayFromInstanceMementoOntoStack(ilgen, property.PropertyType, property.Name);

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}