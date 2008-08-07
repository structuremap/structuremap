using System;
using System.Reflection;
using System.Reflection.Emit;
using StructureMap.Pipeline;

namespace StructureMap.Emitting.Parameters
{
    /// <summary>
    /// Abstract class to provide a template for emitting the instructions for retrieving
    /// one constructor function argument from an InstanceMemento and feeding into the 
    /// constructor function
    /// </summary>
    public abstract class ParameterEmitter
    {
        protected void cast(ILGenerator ilgen, Type parameterType)
        {
			//NOTE: According to the docs, Unbox_Any, when called on a ref type, will just do a Castclass
			//      but it's probably better to err on the side of being explicit rather than relying 
			//      on non-obvious side effects

			if( parameterType.IsValueType )
			{
				ilgen.Emit(OpCodes.Unbox_Any, parameterType);
			}
			else
			{
				ilgen.Emit(OpCodes.Castclass, parameterType);
			}
            
        }

        public abstract void MandatorySetter(ILGenerator ilgen, PropertyInfo property);

        public void OptionalSetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, property.Name);
            ilgen.Emit(OpCodes.Callvirt, Methods.HAS_PROPERTY);
            ilgen.Emit(OpCodes.Ldc_I4_0);
            ilgen.Emit(OpCodes.Ceq);
            ilgen.Emit(OpCodes.Stloc_2);
            ilgen.Emit(OpCodes.Ldloc_2);

            Label label = ilgen.DefineLabel();

            ilgen.Emit(OpCodes.Brtrue_S, label);

            MandatorySetter(ilgen, property);

            ilgen.Emit(OpCodes.Nop);

            ilgen.MarkLabel(label);
        }
    }
}