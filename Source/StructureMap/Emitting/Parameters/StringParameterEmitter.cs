using System.Reflection;
using System.Reflection.Emit;
using StructureMap.Pipeline;

namespace StructureMap.Emitting.Parameters
{
    /// <summary>
    /// Implementation of ParameterEmitter for string type constructor arguments
    /// </summary>
    public class StringParameterEmitter : ParameterEmitter
    {
        public void Ctor(ILGenerator ilgen, ParameterInfo parameter)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, parameter.Name);
            ilgen.Emit(OpCodes.Callvirt, Methods.GET_PROPERTY);
        }

        public override void MandatorySetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, property.Name);
            ilgen.Emit(OpCodes.Callvirt, Methods.GET_PROPERTY);
            ilgen.Emit(OpCodes.Callvirt, property.GetSetMethod());
        }
    }
}