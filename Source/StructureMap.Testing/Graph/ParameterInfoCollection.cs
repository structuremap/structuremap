using System.Collections;
using System.Reflection;

namespace StructureMap.Testing.Graph
{
    public class ParameterInfoCollection
    {
        private readonly Hashtable _parameters;

        public ParameterInfoCollection(ConstructorInfo constructor)
        {
            _parameters = new Hashtable();
            foreach (ParameterInfo param in constructor.GetParameters())
            {
                _parameters.Add(param.Name, param);
            }
        }

        public ParameterInfo this[string name]
        {
            get { return _parameters[name] as ParameterInfo; }
        }
    }
}