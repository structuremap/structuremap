using System;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public class ProfileExpression : IExpression
    {
        public void Configure(PluginGraph graph)
        {
            throw new NotImplementedException();
        }

        public IExpression[] ChildExpressions
        {
            get { throw new NotImplementedException(); }
        }
    }
}
