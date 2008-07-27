using System.Reflection;
using System.Reflection.Emit;
using StructureMap.Emitting.Parameters;
using StructureMap.Graph;

namespace StructureMap.Emitting
{
    public class ArgumentEmitter : IArgumentVisitor
    {
        private readonly ChildParameterEmitter _child = new ChildParameterEmitter();
        private readonly ChildArrayParameterEmitter _childArray = new ChildArrayParameterEmitter();
        private readonly EnumParameterEmitter _enum = new EnumParameterEmitter();
        private readonly PrimitiveParameterEmitter _primitive = new PrimitiveParameterEmitter();
        private readonly StringParameterEmitter _string = new StringParameterEmitter();
        private readonly ILGenerator ilgen;


        public ArgumentEmitter(ILGenerator ilgen)
        {
            this.ilgen = ilgen;
        }

        #region IArgumentVisitor Members

        public void PrimitiveSetter(PropertyInfo property, bool isMandatory)
        {
            if (!isMandatory) return;

            _primitive.Setter(ilgen, property);
        }

        public void StringSetter(PropertyInfo property, bool isMandatory)
        {
            if (!isMandatory) return;

            _string.Setter(ilgen, property);
        }

        public void EnumSetter(PropertyInfo property, bool isMandatory)
        {
            if (!isMandatory) return;

            _enum.Setter(ilgen, property);
        }

        public void ChildSetter(PropertyInfo property, bool isMandatory)
        {
            if (!isMandatory) return;

            _child.Setter(ilgen, property);
        }

        public void ChildArraySetter(PropertyInfo property, bool isMandatory)
        {
            if (!isMandatory) return;

            _childArray.Setter(ilgen, property);
        }

        public void PrimitiveParameter(ParameterInfo parameter)
        {
            _primitive.Ctor(ilgen, parameter);
        }

        public void StringParameter(ParameterInfo parameter)
        {
            _string.Ctor(ilgen, parameter);
        }

        public void EnumParameter(ParameterInfo parameter)
        {
            _enum.Ctor(ilgen, parameter);
        }

        public void ChildParameter(ParameterInfo parameter)
        {
            _child.Ctor(ilgen, parameter);
        }

        public void ChildArrayParameter(ParameterInfo parameter)
        {
            _childArray.Ctor(ilgen, parameter);
        }

        #endregion
    }
}