using NMock;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class NormalGraphBuilderTester
    {
        [Test]
        public void ScopeIsUsedToCreateTheInterceptionChain()
        {
            InstanceScope theScope = InstanceScope.PerRequest;
            InterceptionChain chain = new InterceptionChain();
            DynamicMock builderMock = new DynamicMock(typeof (IInterceptorChainBuilder));
            builderMock.ExpectAndReturn("Build", chain, theScope);

            NormalGraphBuilder graphBuilder =
                new NormalGraphBuilder((IInterceptorChainBuilder) builderMock.MockInstance);

            TypePath typePath = new TypePath(GetType());


            graphBuilder.AddPluginFamily(typePath, "something", new string[0], theScope);

            PluginFamily family = graphBuilder.PluginGraph.PluginFamilies[GetType()];

            Assert.AreSame(chain, family.InterceptionChain);

            builderMock.Verify();
        }

        [Test]
        public void AddProfile()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder();
            string profileName = "blue";

            graphBuilder.AddProfile(profileName);

            InstanceDefaultManager defaultManager = graphBuilder.DefaultManager;
            Assert.AreEqual(1, defaultManager.Profiles.Length);
            Assert.AreEqual(profileName, defaultManager.Profiles[0].ProfileName);
        }

        [Test]
        public void AddDefaultForAProfile()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder();
            string profileName = "blue";
            string theTypeName = "the name of the type";
            string theKey = "Key1";

            graphBuilder.AddProfile(profileName);
            graphBuilder.OverrideProfile(theTypeName, theKey);

            Profile profile = graphBuilder.DefaultManager.Profiles[0];
            Assert.AreEqual(1, profile.Count);
            Assert.AreEqual(theTypeName, profile.Defaults[0].PluginTypeName);
            Assert.AreEqual(theKey, profile.Defaults[0].DefaultKey);
        }


        [Test]
        public void AddMachineWithExistingProfile()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder();
            string theMachineName = "some machine";
            string theProfileName = "some profile";
            graphBuilder.AddProfile(theProfileName);
            graphBuilder.AddMachine(theMachineName, theProfileName);

            Assert.AreEqual(1, graphBuilder.DefaultManager.MachineOverrides.Length);
            MachineOverride machine = graphBuilder.DefaultManager.MachineOverrides[0];
            Assert.AreEqual(theMachineName, machine.MachineName);
            Assert.AreEqual(theProfileName, machine.ProfileName);
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  195\nThe Profile some profile referenced by Machine some machine does not exist"
             )]
        public void AddMachineWithProfileThatDoesNotExist()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder();
            string theMachineName = "some machine";
            string theProfileName = "some profile";

            graphBuilder.AddMachine(theMachineName, theProfileName);
        }

        [Test]
        public void AddMachineWithoutProfile()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder();
            string theMachineName = "some machine";

            graphBuilder.AddMachine(theMachineName, string.Empty);

            Assert.AreEqual(1, graphBuilder.DefaultManager.MachineOverrides.Length);
            MachineOverride machine = graphBuilder.DefaultManager.MachineOverrides[0];
            Assert.AreEqual(theMachineName, machine.MachineName);
            Assert.IsEmpty(machine.ProfileName);
        }

        [Test]
        public void AddAnOverrideToMachine()
        {
            NormalGraphBuilder graphBuilder = new NormalGraphBuilder();
            string theMachineName = "some machine";

            graphBuilder.AddMachine(theMachineName, string.Empty);
            graphBuilder.OverrideMachine("some type", "some key");

            MachineOverride machine = graphBuilder.DefaultManager.MachineOverrides[0];
            Assert.AreEqual(new InstanceDefault[] {new InstanceDefault("some type", "some key")}, machine.InnerDefaults);
        }
    }
}