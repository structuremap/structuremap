using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;

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
            get { return ToDescription(_constructor); }
        }

        public static string ToDescription(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            var paramList = parameters.Select(x => {
                if (x.ParameterType.IsSimple())
                {
                    return "{0} {1}".ToFormat(x.ParameterType.GetTypeName(), x.Name);
                }
                else
                {
                    if (parameters.Where(p => p.ParameterType == x.ParameterType).Count() > 1)
                    {
                        return "{0} {1}".ToFormat(x.ParameterType.GetTypeName(), x.Name);
                    }
                    else
                    {
                        return x.ParameterType.GetTypeName();
                    }
                }
            }).ToArray();



            return "new {0}({1})".ToFormat(constructor.DeclaringType.GetTypeName(), string.Join(", ", paramList));
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