using System;

namespace StructureMap.Building
{
    public class LiteralPlan<T> : IBuildPlan
    {
        private readonly T _object;
        private readonly string _description;

        public LiteralPlan(T @object, string description)
        {
            _object = @object;
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        public Delegate ToDelegate()
        {
            Func<IContext, T> func = c => _object;
            return func;
        }
    }
}