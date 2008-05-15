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
            
            //ilgen.Emit(OpCodes.Ldstr, argumentType.GetElementType().AssemblyQualifiedName);
            ilgen.Emit(OpCodes.Ldtoken, argumentType.GetElementType());
            MethodInfo method = typeof(Type).GetMethod("GetTypeFromHandle");
            ilgen.Emit(OpCodes.Call, method);
            
            ilgen.Emit(OpCodes.Ldarg_1);

            ilgen.Emit(OpCodes.Ldstr, argumentName);
            callInstanceMemento(ilgen, "GetChildrenArray");

            MethodInfo methodCreateInstanceArray = (typeof (StructureMap.Pipeline.IBuildSession).GetMethod("CreateInstanceArray"));
            ilgen.Emit(OpCodes.Callvirt, methodCreateInstanceArray);
            cast(ilgen, argumentType);
        }

        public void Setter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);
            putChildArrayFromInstanceMementoOntoStack(ilgen, property.PropertyType, property.Name);

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}