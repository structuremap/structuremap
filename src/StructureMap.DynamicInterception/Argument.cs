using Castle.DynamicProxy;
using System.Reflection;

namespace StructureMap.DynamicInterception
{
    internal class Argument : IArgument
    {
        private readonly IInvocation _invocation;
        private readonly int _index;
        private object _value;

        public Argument(IInvocation invocation, int index, object value, ParameterInfo parameterInfo,
            ParameterInfo instanceParameterInfo)
        {
            _invocation = invocation;
            _index = index;
            _value = value;
            ParameterInfo = parameterInfo;
            InstanceParameterInfo = instanceParameterInfo;
        }

        public object Value
        {
            get => _value;
            set
            {
                _invocation.SetArgumentValue(_index, value);
                _value = value;
            }
        }

        public ParameterInfo ParameterInfo { get; }

        public ParameterInfo InstanceParameterInfo { get; }
    }
}