using System;
using System.Linq;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class FuncBuildByNamePolicy : IFamilyPolicy
    {
        public PluginFamily Build(Type type)
        {
            if (!type.GetTypeInfo().IsGenericType) return null;

            var basicType = type.GetGenericTypeDefinition();

            if (basicType == typeof (Func<,>) && type.GetGenericArguments().First() == typeof (string))
            {
                var family = new PluginFamily(type);
                var typeToBeBuilt = type.GetGenericArguments().Last();
                var @default =
                    Activator.CreateInstance(typeof (FuncByNameInstance<>).MakeGenericType(typeToBeBuilt)) as Instance;

                family.SetDefault(@default);

                return family;
            }

            return null;
        }

        public bool AppliesToHasFamilyChecks
        {
            get
            {
                return true;
            }
        }
    }

    public class FuncByNameInstance<T> : LambdaInstance<Func<string, T>>
    {
        public FuncByNameInstance() : base("Builder by name for " + typeof(T).GetTypeName(), c => name => c.GetInstance<T>(name))
        {
        }
    }
}