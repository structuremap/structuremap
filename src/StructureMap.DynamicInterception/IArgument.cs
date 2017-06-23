using System.Reflection;

namespace StructureMap.DynamicInterception
{
    public interface IArgument
    {
        object Value { get; set; }

        ParameterInfo ParameterInfo { get; }

        ParameterInfo InstanceParameterInfo { get; }
    }
}