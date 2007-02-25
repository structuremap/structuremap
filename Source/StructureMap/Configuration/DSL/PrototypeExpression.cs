using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class PrototypeExpression : IExpression
    {
        public void Configure(PluginGraph graph)
        {
            throw new NotImplementedException();
        }

        public IExpression[] ChildExpressions
        {
            get { return new IExpression[0]; }
        }

        public void WithName(string instanceKey)
        {
            
        }
    }
}
