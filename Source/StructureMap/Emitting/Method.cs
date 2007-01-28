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
        private MethodBuilder methodBuilder;
        private ILGenerator ilgen;

        protected Method()
        {
        }

        private const MethodAttributes PublicOverride =
            MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Final;

        internal void Attach(TypeBuilder newTypeBuilder)
        {
            methodBuilder =
                newTypeBuilder.DefineMethod(MethodName, PublicOverride, CallingConventions.Standard, ReturnType,
                                            ArgumentList);
            ilgen = methodBuilder.GetILGenerator();
        }

        public abstract string MethodName { get; }

        public abstract Type[] ArgumentList { get; }

        public void Build()
        {
            Generate(ilgen);
        }

        protected abstract void Generate(ILGenerator ilgen);

        public abstract Type ReturnType { get; }


        protected void CallMethod(MethodInfo methodInfo)
        {
            ilgen.EmitCall(OpCodes.Call, methodInfo, null);
            ilgen.Emit(OpCodes.Ldloc_0);
        }
    }
}