using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Emitting
{
    public class ConstructorEmitter : IPluginArgumentVisitor
    {
        private readonly ILGenerator _ilgen;

        public ConstructorEmitter(ILGenerator ilgen)
        {
            _ilgen = ilgen;
        }


        public void Primitive(string name)
        {
            throw new NotImplementedException();
        }

        public void Child(string name, Type childType)
        {
            throw new NotImplementedException();
        }

        public void ChildArray(string name, Type childType)
        {
            throw new NotImplementedException();
        }

        protected void callInstanceMemento(ILGenerator ilgen, string methodName)
        {
            MethodInfo _method = typeof(IConfiguredInstance).GetMethod(methodName);
            ilgen.Emit(OpCodes.Callvirt, _method);
        }

        protected void cast(ILGenerator ilgen, Type parameterType)
        {
            ilgen.Emit(OpCodes.Castclass, parameterType);
        }
    }
}
