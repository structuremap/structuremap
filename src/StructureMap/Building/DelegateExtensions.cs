using System;

namespace StructureMap.Building
{
    public static class DelegateExtensions
    {
        public static Func<IBuildSession, T> BuilderOf<T>(this Delegate @delegate)
        {
            return @delegate.As<Func<IBuildSession, T>>();
        }

        public static Func<IBuildSession, T> ToDelegate<T>(this IBuildPlan plan)
        {
            return plan.ToDelegate().As<Func<IBuildSession, T>>();
        }
    }
}