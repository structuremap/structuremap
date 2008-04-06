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
        protected override bool canProcess(Type parameterType)
        {
            return (!parameterType.IsPrimitive && !parameterType.IsArray);
        }

        protected override void generate(ILGenerator ilgen, ParameterInfo parameter)
        {
            Type parameterType = parameter.ParameterType;
            string parameterName = parameter.Name;
            string fullName = parameterType.AssemblyQualifiedName;


            putChildObjectOnStack(ilgen, parameterName, fullName, parameterType);
        }

        private void putChildObjectOnStack(ILGenerator ilgen, string parameterName, string fullName, Type parameterType)
        {
            ilgen.Emit(OpCodes.Ldarg_1);
            ilgen.Emit(OpCodes.Ldstr, parameterName);
            ilgen.Emit(OpCodes.Ldstr, fullName);
            ilgen.Emit(OpCodes.Ldarg_2);

            callInstanceMemento(ilgen, "GetChild");
            cast(ilgen, parameterType);
        }

        protected override void generateSetter(ILGenerator ilgen, PropertyInfo property)
        {
            ilgen.Emit(OpCodes.Ldloc_0);

            putChildObjectOnStack(ilgen, property.Name, property.PropertyType.AssemblyQualifiedName,
                                  property.PropertyType);

            MethodInfo method = property.GetSetMethod();
            ilgen.Emit(OpCodes.Callvirt, method);
        }
    }
}