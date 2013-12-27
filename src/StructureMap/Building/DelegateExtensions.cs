using System;

namespace StructureMap.Building
{
    public static class DelegateExtensions
    {
        public static Func<IBuildSession, T> BuilderOf<T>(this Delegate @delegate)
        {
            return @delegate.As<Func<IBuildSession, T>>();
        }

        public static T Build<T>(this IBuildPlan plan, IBuildSession session) where T : class
        {
            return plan.Build(session).As<T>();
        }
    }
}