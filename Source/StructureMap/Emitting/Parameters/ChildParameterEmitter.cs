using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting.Parameters
{
    /// <summary>
    /// Implementation of ParameterEmitter for a non-primitive, non-array
    /// constructor argument
    /// </summary>
    public class ChildParameterEmitter : ParameterEmitter
    {
        public void Ctor(ILGenerator ilgen, ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            string parameterName = parameter.Name;

            putChildObjectOnStack(ilgen, parameterName, parameterType);
        }

        private void putChildObjectOnStack(ILGenerator ilgen, string parameterName, Type parameterType)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, parameterName);

            ilgen.Emit(OpCodes.Ldtoken, parameterType);

            ilgen.Emit(OpCodes.Call, Methods.GET_TYPE_FROM_HANDLE);

            ilgen.Emit(OpCodes.Ldarg_2);
            ilgen.Emit(OpCodes.Callvirt, Methods.GET_CHILD);

            cast(ilgen, parameterType);
        }

        public override void MandatorySetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);

            putChildObjectOnStack(ilgen, property.Name, property.PropertyType);

            MethodInfo method = property.GetSetMethod();

            if (method == null)
            {
                string message = string.Format("Could not find a Setter for property {0} of type {1}", property.Name,
                                               property.DeclaringType.FullName);
                throw new ApplicationException(message);
            }

            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}