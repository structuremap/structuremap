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

        private void addSetter(ParameterEmitter emitter, PropertyInfo property, bool isMandatory )
        {
            if (isMandatory)
            {
                emitter.MandatorySetter(ilgen, property);
            }
            else
            {
                emitter.OptionalSetter(ilgen, property);
            }
        }

        #region IArgumentVisitor Members

        public void PrimitiveSetter(PropertyInfo property, bool isMandatory)
        {
            addSetter(_primitive, property, isMandatory);
        }

        public void StringSetter(PropertyInfo property, bool isMandatory)
        {
            addSetter(_string, property, isMandatory);
        }

        public void EnumSetter(PropertyInfo property, bool isMandatory)
        {
            addSetter(_enum, property, isMandatory);
        }

        public void ChildSetter(PropertyInfo property, bool isMandatory)
        {
            addSetter(_child, property, isMandatory);
        }

        public void ChildArraySetter(PropertyInfo property, bool isMandatory)
        {
            addSetter(_childArray, property, isMandatory);
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