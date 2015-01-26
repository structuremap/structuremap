using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_255_InstanceAttribute_Implementation
    {
        [Test]
        public void smart_instance_respects_the_name_of_the_inner()
        {
            new SmartInstance<ClassWithInstanceAttributes>().Name.ShouldEqual("SteveBono");
        }

        [Test]
        public void attribute_should_alter_the_concrete_instance_in_explicit_config()
        {
            new SmartInstance<ClassWithInstanceAttributes>().Name.ShouldEqual("SteveBono");
            
            var container = new Container(x => {
                x.For<IBase>().Use<ClassWithInstanceAttributes>();
            });
            
            container.GetInstance<IBase>("SteveBono").ShouldNotBeNull();
            
            container.Model.Find<IBase>("SteveBono").ShouldNotBeNull();
        }

        [Test]
        public void attribute_should_alter_the_concrete_instance_in_scanning()
        {
            var container = new Container(x => {
                x.Scan(_ => {
                    _.TheCallingAssembly();
                    _.AddAllTypesOf<IBase>();
                });
            });

            container.GetInstance<IBase>("SteveBono").ShouldNotBeNull();

            container.Model.Find<IBase>("SteveBono").ShouldNotBeNull();
        }
    }

    public class PluggableAttribute : InstanceAttribute
    {
        public string ConcreteKey { get; set; }

        public PluggableAttribute(string concreteKey)
        {
            ConcreteKey = concreteKey;
        }

        public override void Alter(IConfiguredInstance instance)
        {
            instance.Name = ConcreteKey;
        }
    }

    public interface IBase
    {
    }

    [Pluggable("SteveBono")]
    public class ClassWithInstanceAttributes : IBase
    {
    }
}