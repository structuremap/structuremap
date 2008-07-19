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
        protected void callInstanceMemento(ILGenerator ilgen, string methodName)
        {
            MethodInfo _method = typeof (IConfiguredInstance).GetMethod(methodName);
            ilgen.Emit(OpCodes.Callvirt, _method);
        }

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
    }
}