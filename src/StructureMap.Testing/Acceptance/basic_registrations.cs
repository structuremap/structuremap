using System;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using StructureMap.Testing.GenericWidgets;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class basic_registrations
    {
        [Test]
        public void blows_up_if_no_default()
        {
            var container = new Container(_ =>
            {
                // Register
                _.For<IWidget>().Add<AWidget>();
                _.For<IWidget>().Add<BWidget>();
            });
        }
    }

    // SAMPLE: SettingDefaults
    public class SettingDefaults : Registry
    {
        public SettingDefaults()
        {
            // If you know the plugin type and its a closed type
            // you can use this syntax
            For<IWidget>().Use<DefaultWidget>();

            // By Lambda
            For<IWidget>().Use(() => new DefaultWidget());

            // Pre-existing object
            For<IWidget>().Use(new AWidget());

            // This is rare now, but still valid
            For<IWidget>().Add<AWidget>().Named("A");
            For<IWidget>().Add<BWidget>().Named("B");
            For<IWidget>().Use("A"); // makes AWidget the default

            // Also rare, but you can supply an Instance object
            // yourself for special needs
            For<IWidget>().UseInstance(new MySpecialInstance());
            
            // If you're registering an open generic type
            // or you just have Type objects, use this syntax
            For(typeof (IService<>)).Use(typeof (Service<>));

            // This is occasionally useful for generic types
            For(typeof (IService<>)).Use(new MySpecialInstance());
        }
    }
    // ENDSAMPLE

    // SAMPLE: AdditionalRegistrations
    public class AdditionalRegistrations : Registry
    {
        public AdditionalRegistrations()
        {
            // If you know the plugin type and its a closed type
            // you can Add this syntax
            For<IWidget>().Add<DefaultWidget>();

            // By Lambda
            For<IWidget>().Add(() => new DefaultWidget());

            // Pre-existing object
            For<IWidget>().Add(new AWidget());

            // This is rare now, but still valid
            For<IWidget>().Add<AWidget>().Named("A");
            For<IWidget>().Add<BWidget>().Named("B");
            For<IWidget>().Add("A"); // makes AWidget the default

            // Also rare, but you can supply an Instance object
            // yourself for special needs
            For<IWidget>().AddInstance(new MySpecialInstance());

            // If you're registering an open generic type
            // or you just have Type objects, Add this syntax
            For(typeof(IService<>)).Add(typeof(Service<>));

            // This is occasionally Addful for generic types
            For(typeof(IService<>)).Add(new MySpecialInstance());
        }
    }
    // ENDSAMPLE


    public class MySpecialInstance : Instance
    {
        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        public override Type ReturnedType
        {
            get { throw new NotImplementedException(); }
        }
    }
}