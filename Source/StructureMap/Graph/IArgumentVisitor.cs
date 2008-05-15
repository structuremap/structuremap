using System.Reflection;

namespace StructureMap.Graph
{
    public interface IArgumentVisitor
    {
        void PrimitiveSetter(PropertyInfo property);
        void StringSetter(PropertyInfo property);
        void EnumSetter(PropertyInfo property);
        void ChildSetter(PropertyInfo property);
        void ChildArraySetter(PropertyInfo property);

        void PrimitiveParameter(ParameterInfo parameter);
        void StringParameter(ParameterInfo parameter);
        void EnumParameter(ParameterInfo parameter);
        void ChildParameter(ParameterInfo parameter);
        void ChildArrayParameter(ParameterInfo parameter);
    }
}