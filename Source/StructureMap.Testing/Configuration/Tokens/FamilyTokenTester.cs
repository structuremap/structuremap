using System;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Configuration.Mementos;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.Tokens
{
    [TestFixture]
    public class FamilyTokenTester
    {
        [Test]
        public void CreateImplicitFamily()
        {
            PluginFamily family = new PluginFamily(typeof (IGateway));
            family.DefinitionSource = DefinitionSource.Implicit;

            FamilyToken token = FamilyToken.CreateImplicitFamily(family);

            // Type IGateway has a default instance key defined on the PluginFamilyAttribute
            FamilyToken expected = new FamilyToken(typeof (IGateway), "Default", new string[0]);
            expected.DefinitionSource = DefinitionSource.Implicit;

            Assert.AreEqual(expected, token);
            Assert.AreEqual(InstanceScope.PerRequest, token.Scope);
            Assert.AreEqual(0, token.Interceptors.Length);
        }


        [Test]
        public void CreateImplicitFamilyWithScopeOtherThanPerRequest()
        {
            PluginFamily family = new PluginFamily(typeof (ImplicitFamilyTarget));
            family.DefinitionSource = DefinitionSource.Implicit;

            FamilyToken token = FamilyToken.CreateImplicitFamily(family);

            Assert.AreEqual(InstanceScope.HttpContext, token.Scope);

            Assert.AreEqual(1, token.Interceptors.Length);
            InterceptorInstanceToken interceptor = (InterceptorInstanceToken) token.Interceptors[0];
            Assert.AreEqual("HttpContext", interceptor.InstanceKey);
        }

        [Test]
        public void DefaultKeyDoesNotExist()
        {
            PluginGraphReport report = new PluginGraphReport();

            PluginFamily family = new PluginFamily(typeof (IGateway));
            family.DefaultInstanceKey = "something that does not exist";
            MemoryMementoSource source = new MemoryMementoSource();

            source.AddMemento(new MemoryInstanceMemento("Default", "one"));
            source.AddMemento(new MemoryInstanceMemento("Stubbed", "two"));

            family.Source = source;

            FamilyToken token = new FamilyToken(typeof (IGateway), family.DefaultInstanceKey, new string[0]);
            report.AddFamily(token);
            token.ReadInstances(family, report);

            Assert.AreEqual(string.Empty, family.DefaultInstanceKey,
                            "Imperative that the family.DefaultInstanceKey get set to string.empty");

            Problem expected = new Problem(ConfigurationConstants.CONFIGURED_DEFAULT_KEY_CANNOT_BE_FOUND, string.Empty);
            Assert.AreEqual(new Problem[] {expected}, token.Problems);
        }

        [Test]
        public void ReadExternalInstancesFromSourceAllSuccess()
        {
            PluginGraphReport report = new PluginGraphReport();

            PluginFamily family = new PluginFamily(typeof (IGateway));
            MemoryMementoSource source = new MemoryMementoSource();

            MemoryInstanceMemento memento1 = new MemoryInstanceMemento("Default", "one");
            memento1.DefinitionSource = DefinitionSource.Implicit;
            source.AddExternalMemento(memento1);
            MemoryInstanceMemento memento2 = new MemoryInstanceMemento("Stubbed", "two");
            source.AddExternalMemento(memento2);
            memento2.DefinitionSource = DefinitionSource.Implicit;

            family.Source = source;

            FamilyToken token = new FamilyToken(family.PluginType, null, new string[0]);
            report.AddFamily(token);
            token.ReadInstances(family, report);

            Assert.AreEqual(2, token.Instances.Length);

            Assert.AreEqual(DefinitionSource.Implicit, token.Instances[0].Source);
            Assert.AreEqual(DefinitionSource.Implicit, token.Instances[1].Source);
        }

        [Test]
        public void ReadInstancesFindsImplicitInstances()
        {
            PluginGraphReport report = new PluginGraphReport();

            // DefaultGateway has no constructor arguments, so it should be available as 
            // an implicit instance
            PluginFamily family = new PluginFamily(typeof (IGateway));
            MemoryMementoSource source = new MemoryMementoSource();
            family.Plugins.Add(typeof (DefaultGateway), "Implicit");

            FamilyToken token = new FamilyToken(family.PluginType, null, new string[0]);
            report.AddFamily(token);
            token.ReadInstances(family, report);

            Assert.AreEqual(1, token.Instances.Length);
            InstanceToken instance = token.FindInstance("Implicit");

            Assert.IsNotNull(instance);
            Assert.AreEqual(DefinitionSource.Implicit, instance.Source);
            Assert.AreEqual("Implicit", instance.InstanceKey);
            Assert.AreEqual("Implicit", instance.ConcreteKey);
        }

        [Test]
        public void ReadInstancesFromSourceAllSuccess()
        {
            PluginGraphReport report = new PluginGraphReport();

            PluginFamily family = new PluginFamily(typeof (IGateway));
            MemoryMementoSource source = new MemoryMementoSource();

            source.AddMemento(new MemoryInstanceMemento("Default", "one"));
            source.AddMemento(new MemoryInstanceMemento("Stubbed", "two"));

            family.Source = source;

            FamilyToken token = new FamilyToken(family.PluginType, null, new string[0]);
            report.AddFamily(token);

            token.ReadInstances(family, report);

            Assert.AreEqual(2, token.Instances.Length);
        }

        [Test]
        public void ReadInstancesMementoSourceFails()
        {
            PluginGraphReport report = new PluginGraphReport();

            PluginFamily family = new PluginFamily(typeof (IGateway));
            family.Source = new BadMementoSource();

            FamilyToken token = new FamilyToken(family.PluginType, string.Empty, new string[0]);
            token.ReadInstances(family, report);

            Problem[] expected = new Problem[]
                {
                    new Problem(ConfigurationConstants.MEMENTO_SOURCE_CANNOT_RETRIEVE, string.Empty),
                    new Problem(ConfigurationConstants.CONFIGURED_DEFAULT_KEY_CANNOT_BE_FOUND, string.Empty)
                };


            Assert.AreEqual(expected, token.Problems);
        }
    }

    [PluginFamily(Scope = InstanceScope.HttpContext)]
    public interface ImplicitFamilyTarget
    {
    }

    public class BadMementoSource : MementoSource
    {
        public override string Description
        {
            get { throw new NotImplementedException(); }
        }

        protected override InstanceMemento[] fetchInternalMementos()
        {
            throw new NotImplementedException();
        }

        protected override bool containsKey(string instanceKey)
        {
            throw new NotImplementedException();
        }

        protected override InstanceMemento retrieveMemento(string instanceKey)
        {
            throw new NotImplementedException();
        }
    }
}