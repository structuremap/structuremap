using System.Reflection;

namespace StructureMap.Graph
{
    public interface IArgumentVisitor
    {
        void PrimitiveSetter(PropertyInfo property, bool isMandatory);
        void StringSetter(PropertyInfo property, bool isMandatory);
        void EnumSetter(PropertyInfo property, bool isMandatory);
        void ChildSetter(PropertyInfo property, bool isMandatory);
        void ChildArraySetter(PropertyInfo property, bool isMandatory);

        void PrimitiveParameter(ParameterInfo parameter);
        void StringParameter(ParameterInfo parameter);
        void EnumParameter(ParameterInfo parameter);
        void ChildParameter(ParameterInfo parameter);
        void ChildArrayParameter(ParameterInfo parameter);
    }
}