using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace StructureMap.Testing.Graph
{
    public class ParameterInfoCollection
    {
        private readonly Dictionary<string, ParameterInfo> _parameters;

        public ParameterInfoCollection(ConstructorInfo constructor)
        {
            _parameters = new Dictionary<string, ParameterInfo>();
            foreach (var param in constructor.GetParameters())
            {
                _parameters.Add(param.Name, param);
            }
        }

        public ParameterInfo this[string name] => _parameters[name];
    }
}