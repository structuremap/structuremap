using System.Collections;
using System.Data;
using System.Runtime.CompilerServices;
using StructureMap.DataAccess.Parameters;

namespace StructureMap.DataAccess
{
    public class ParameterCollection : IEnumerable
    {
        private Hashtable _parameters = new Hashtable();

        public ParameterCollection()
        {
        }

        public ParameterCollection(IDataParameterCollection parameters)
        {
            foreach (IDataParameter innerParameter in parameters)
            {
                Parameter parameter = new Parameter(innerParameter);
                AddParameter(parameter);
            }
        }

        public ParameterCollection(IParameter[] parameters)
        {
            foreach (IParameter parameter in parameters)
            {
                AddParameter(parameter);
            }
        }

        public void AddParameter(IParameter parameter)
        {
            _parameters[parameter.ParameterName.ToUpper()] = parameter;
        }

        public IEnumerator GetEnumerator()
        {
            return _parameters.Values.GetEnumerator();
        }

        [IndexerName("Parameter")]
        public IParameter this[string parameterName]
        {
            get { return _parameters[parameterName.ToUpper()] as IParameter; }
            set { _parameters[parameterName.ToUpper()] = value; }
        }

        public int Count
        {
            get { return _parameters.Count; }
        }
    }
}