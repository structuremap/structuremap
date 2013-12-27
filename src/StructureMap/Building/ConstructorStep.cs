using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class ConstructorStep
    {
        private readonly ConstructorInfo _constructor;
        private readonly IList<IBuildStep> _arguments = new List<IBuildStep>(); 

        public ConstructorStep(ConstructorInfo constructor)
        {
            _constructor = constructor;
        }

        public void Add(IBuildStep argument)
        {
            _arguments.Add(argument);
        }

        public IEnumerable<IBuildStep> Arguments
        {
            get { return _arguments; }
        }

        public ConstructorInfo Constructor
        {
            get { return _constructor; }
        }

        public string Description
        {
            get { return _constructor.ToString(); } // TODO -- much more here
        }

        public NewExpression ToExpression()
        {
            return Expression.New(_constructor, _arguments.Select(x => x.ToExpression()));
        }


    }
}