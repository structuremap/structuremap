namespace StructureMap.Docs.samples.glossary
{
    internal class glossary
    {
        public void container()
        {
// SAMPLE: glossary-container
            var container = new Container(c => { c.AddRegistry<FooBarRegistry>(); });
// ENDSAMPLE
        }

        public void nested_container()
        {
            var someExistingContainer = new Container();
// SAMPLE: glossary-nested-container
            using (var nested = someExistingContainer.GetNestedContainer())
            {
                // pull other objects from the nested container and do work with those services
                var service = nested.GetInstance<IService>();
                service.DoSomething();
            }
// ENDSAMPLE
        }

        public void plugintype_and_pluggedtype()
        {
// SAMPLE: glossary-pluggintype-and-plugged-type
//For<PLUGINTYPE>().Use<PLUGGEDTYPE>()

            var container = new Container(c => { c.AddRegistry<FooRegistry>(); });

            container.GetInstance<IFoo>();

//container.GetInstance<PLUGINTYPE>()
// ENDSAMPLE
        }

        public void pluginfamilly()
        {
// SAMPLE: glossary-pluginfamily
            var container = new Container(c =>
            {
                c.For<IFoo>().Use<Foo>();
                c.For<IFoo>().Add<SomeOtherFoo>();
            });
// ENDSAMPLE
        }
    }
}