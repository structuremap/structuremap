using System;
using System.Diagnostics;

namespace StructureMap.Docs.samples.quickstart
{
    internal class resolving_instances
    {
        private readonly IContainer container = new Container();

        public void get_instance_from_container_1()
        {
// SAMPLE: quickstart-resolve-strongly-typed-instance
            var fooInstance = container.GetInstance<IFoo>();
// ENDSAMPLE
        }

        public void get_instance_from_container_2()
        {
// SAMPLE: quickstart-resolve-weakly-typed-instance
            var foo = typeof (IFoo);

            var fooInstance = container.GetInstance(foo);
// ENDSAMPLE
        }

        public void resolve_all_instances_of_foo()
        {
// SAMPLE: quickstart-resolve-all-instances-of-foo
            var fooInstances = container.GetAllInstances<IFoo>();
// ENDSAMPLE
        }

        public void resolve_unknown_instance_blah()
        {
// SAMPLE: quickstart-resolve-unknown-instance-blah
            var blahInstance = container.TryGetInstance<IBlah>();

            Debug.Assert(blahInstance != null, String.Format("no default instance for {0}", typeof (IBlah).FullName));
// ENDSAMPLE
        }

        public void resolve_concrete_types()
        {
// SAMPLE: quickstart-resolve-concrete-types
            var container = new Container();
            var weather1 = container.GetInstance<Weather>();

            var weather2 = container.GetInstance<Weather>();
            weather2 = container.GetInstance<Weather>(); //short version for above.
// ENDSAMPLE
        }
    }
}