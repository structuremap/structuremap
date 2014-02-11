using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class ConstructorStep
    {
        private readonly ConstructorInfo _constructor;
        private readonly List<IDependencySource> _arguments = new List<IDependencySource>();

        public ConstructorStep(ConstructorInfo constructor)
        {
            if (constructor == null) throw new ArgumentNullException("constructor");

            _constructor = constructor;
        }

        public void Add(IDependencySource argument)
        {
            _arguments.Add(argument);
        }

        public IEnumerable<IDependencySource> Arguments
        {
            get { return _arguments; }
        }

        public ConstructorInfo Constructor
        {
            get { return _constructor; }
        }

        public string Description
        {
            get { return _constructor.ToString(); }
        }

        public NewExpression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            return Expression.New(_constructor, _arguments.Select(x => x.ToExpression(session, context)));
        }

        public void Add(IEnumerable<IDependencySource> arguments)
        {
            _arguments.AddRange(arguments);
        }
    }


}