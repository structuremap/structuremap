using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StructureMap.Docs.samples.quickstart
{
    class resolving_instances
    {
        public void get_instance_from_objectfactory_1()
        {
// SAMPLE: quickstart-resolve-strongly-typed-instance
var fooInstance = ObjectFactory.GetInstance<IFoo>();
// ENDSAMPLE
        }

    public void get_instance_from_objectfactory_2()
    {
// SAMPLE: quickstart-resolve-weakly-typed-instance
Type foo = typeof(IFoo);

var fooInstance = ObjectFactory.GetInstance(foo);
// ENDSAMPLE
    }
        
        public void resolve_all_instances_of_foo()
        {
// SAMPLE: quickstart-resolve-all-instances-of-foo
IEnumerable<IFoo> fooInstances = ObjectFactory.GetAllInstances<IFoo>();
// ENDSAMPLE
        }

    public void resolve_unknown_instance_blah()
    {
// SAMPLE: quickstart-resolve-unknown-instance-blah
var blahInstance = ObjectFactory.Container.TryGetInstance<IBlah>();

Debug.Assert(blahInstance != null, String.Format("no default instance for {0}", typeof(IBlah).FullName));
// ENDSAMPLE
    }

        public void resolve_concrete_types()
        {
// SAMPLE: quickstart-resolve-concrete-types
var container = new Container();
var weather1 = container.GetInstance<Weather>();

var weather2 = ObjectFactory.Container.GetInstance<Weather>();
    weather2 = ObjectFactory.GetInstance<Weather>(); //short version for above.
// ENDSAMPLE
        }

    }
}
