using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting.Parameters
{
    /// <summary>
    /// Implementation of ParameterEmitter for string type constructor arguments
    /// </summary>
    public class StringParameterEmitter : ParameterEmitter
    {
        protected override bool canProcess(Type parameterType)
        {
            return (parameterType.Equals(typeof (string)));
        }

        protected override void generate(ILGenerator ilgen, ParameterInfo parameter)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, parameter.Name);
            callInstanceMemento(ilgen, "GetProperty");
        }


        protected override void generateSetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, property.Name);
            callInstanceMemento(ilgen, "GetProperty");

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}