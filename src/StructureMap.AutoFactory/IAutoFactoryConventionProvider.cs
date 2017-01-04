using System.Collections.Generic;
using System.Reflection;

namespace StructureMap.AutoFactory
{
    public interface IAutoFactoryConventionProvider
    {
        // SAMPLE: IAutoFactoryConventionProvider-GetMethodDefinition
        IAutoFactoryMethodDefinition GetMethodDefinition(MethodInfo methodInfo, IList<object> arguments);

        //ENDSAMPLE
    }
}