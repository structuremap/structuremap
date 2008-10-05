using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting.Parameters
{
    /// <summary>
    /// Implementation of ParameterEmitter for primitive types other than strings.
    /// </summary>
    public class PrimitiveParameterEmitter : ParameterEmitter
    {
        public void Ctor(ILGenerator ilgen, ParameterInfo parameter)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, parameter.Name);
            ilgen.Emit(OpCodes.Callvirt, Methods.GET_PROPERTY);
            callParse(parameter.ParameterType, ilgen);
        }

        private void callParse(Type argumentType, ILGenerator ilgen)
        {
            ilgen.Emit(OpCodes.Call, Methods.ParseFor(argumentType));
        }

        public override void MandatorySetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, property.Name);

            ilgen.Emit(OpCodes.Callvirt, Methods.GET_PROPERTY);
            callParse(property.PropertyType, ilgen);

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}