using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public interface IExpression
    {
        void Configure(PluginGraph graph);
        IExpression[] ChildExpressions { get;}
    }
}
