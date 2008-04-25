using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap.Pipeline
{
    public interface ILocationPolicy
    {
        object Build(IInstanceCreator creator, Instance instance);
    }

    //public class DefaultPolicy : ILocationPolicy
    //{
    //    public object Build(IInstanceCreator creator, Instance instance)
    //    {
    //        return instance.Build(creator);
    //    }
    //}
}
