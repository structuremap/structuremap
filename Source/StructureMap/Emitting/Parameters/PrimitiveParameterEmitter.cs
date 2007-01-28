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
        protected override bool canProcess(Type parameterType)
        {
            return (parameterType.IsPrimitive && !parameterType.Equals(typeof (string)));
        }

        protected override void generate(ILGenerator ilgen, ParameterInfo parameter)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, parameter.Name);
            callInstanceMemento(ilgen, "GetProperty");
            callParse(parameter.ParameterType, ilgen);
        }

        private void callParse(Type argumentType, ILGenerator ilgen)
        {
            BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public;
            MethodInfo parseMethod =
                argumentType.GetMethod("Parse", bindingAttr, null, new Type[] {typeof (string)}, null);
            ilgen.Emit(OpCodes.Call, parseMethod);
        }


        protected override void generateSetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, property.Name);

            callInstanceMemento(ilgen, "GetProperty");
            callParse(property.PropertyType, ilgen);

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}