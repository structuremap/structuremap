using System;
using StructureMap.Building;

namespace StructureMap.Pipeline.Lazy
{
    public class FuncFactoryTemplate : Instance
    {
        public override string Description
        {
            get { return "Open Generic Template for Func<>"; }
        }

        // This should never get called because it starts as an open type
        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotSupportedException();
        }

        public override IDependencySource ToBuilder(Type pluginType, Policies policies)
        {
            throw new NotSupportedException();
        }

        public override Type ReturnedType
        {
            get { return typeof (Func<>); }
        }

        public override Instance CloseType(Type[] types)
        {
            var instanceType = typeof (FuncInstance<>).MakeGenericType(types);
            return Activator.CreateInstance(instanceType).As<Instance>();
        }
    }
}