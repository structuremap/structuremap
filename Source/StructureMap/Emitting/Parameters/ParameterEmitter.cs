using System;
using System.Reflection;
using System.Reflection.Emit;
using StructureMap.Graph;
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
            ilgen.Emit(OpCodes.Castclass, parameterType);
        }   
    }
}