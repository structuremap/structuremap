using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting
{
    /// <summary>
    /// Emits the IL for one method
    /// </summary>
    public abstract class Method
    {
        private const MethodAttributes PublicOverride =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Final;

        private ILGenerator ilgen;
        private MethodBuilder methodBuilder;

        public abstract string MethodName { get; }

        public abstract Type[] ArgumentList { get; }
        public abstract Type ReturnType { get; }

        internal void Attach(TypeBuilder newTypeBuilder)
        {
            methodBuilder =
                newTypeBuilder.DefineMethod(MethodName, PublicOverride, CallingConventions.Standard, ReturnType,
                                            ArgumentList);
            ilgen = methodBuilder.GetILGenerator();
        }

        public void Build()
        {
            Generate(ilgen);
        }

        protected abstract void Generate(ILGenerator ilgen);


        protected void CallMethod(MethodInfo methodInfo)
        {
            ilgen.EmitCall(OpCodes.Call, methodInfo, null);
            ilgen.Emit(OpCodes.Ldloc_0);
        }
    }
}