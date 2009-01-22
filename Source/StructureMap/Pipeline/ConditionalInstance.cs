using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.Pipeline
{
    public class ConditionalInstance<T> : ExpressedInstance<ConditionalInstance<T>>
    {
        // Conditional Instance keeps track of zero or more internal Instance
        // objects against a Predicate<IContext> condition
        private readonly List<InstanceCase> _cases = new List<InstanceCase>();
        
        // The "default" Instance to use if none of the conditional predicates
        // are met.  If this is not explicitly defined, the ConditionalInstance
        // will simply look for the default Instance of the desired
        // PluginType
        public Instance _default = new DefaultInstance();


        public ConditionalInstance(Action<ConditionalInstanceExpression> action)
        {
            action(new ConditionalInstanceExpression(this));
        }

        protected override string getDescription()
        {
            return "Conditional Instance of " + typeof (T).FullName;
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            // Find the first InstanceCase that matches the BuildSession/IContext
            var instanceCase = _cases.Find(c => c.Predicate(session));

            // Use the Instance from the InstanceCase if it exists,
            // otherwise, use the "default"
            var instance = instanceCase == null ? _default : instanceCase.Instance;

            // delegate to the chosen Instance
            return instance.Build(pluginType, session);
        }

        public class ConditionalInstanceExpression
        {
            private readonly ConditionalInstance<T> _parent;

            public ConditionalInstanceExpression(ConditionalInstance<T> parent)
            {
                _parent = parent;
            }

            public ThenItExpression<T> If(Predicate<IContext> predicate)
            {
                return new InstanceExpression<T>(i =>
                {
                    var theCase = new InstanceCase() {Instance = i, Predicate = predicate};
                    _parent._cases.Add(theCase);
                });
            }

            public IsExpression<T> TheDefault
            {
                get
                {
                    return new InstanceExpression<T>(i => _parent._default = i);
                }
            }

            
        }

        protected override ConditionalInstance<T> thisInstance
        {
            get { return this; }
        }
    }

    public class InstanceCase
    {
        public Predicate<IContext> Predicate { get; set; }
        public Instance Instance { get; set; }
    }
}