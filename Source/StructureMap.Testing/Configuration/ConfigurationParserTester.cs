using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class ConfigurationParserTester
    {
        private string xml =
            @"
<StructureMap DefaultProfile=""Blue"">
	<Profile Name=""Blue"">
		<Override Type=""StructureMap.Testing.Widget.IWidget"" DefaultKey=""Blue""/>
		<Override Type=""StructureMap.Testing.Widget.Rule"" DefaultKey=""Blue""/>
		<Override Type=""StructureMap.Testing.Widget4.IStrategy"" DefaultKey=""Blue""/>
	</Profile>

	<Profile Name=""Green"">
		<Override Type=""StructureMap.Testing.Widget.IWidget"" DefaultKey=""Green""/>
		<Override Type=""StructureMap.Testing.Widget.Rule"" DefaultKey=""Green""/>
	</Profile>	

	<Machine Name=""GREEN-BOX"" Profile=""Green""/>
	
	<Machine Name=""SERVER"">
		<Override Type=""StructureMap.Testing.Widget.IWidget"" DefaultKey=""Orange""/>
		<Override Type=""StructureMap.Testing.Widget4.IStrategy"" DefaultKey=""Blue""/>
	</Machine>
</StructureMap>";

        private InstanceDefaultManager _defaults;


        [SetUp]
        public void SetUp()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            ConfigurationParser parser = new ConfigurationParser(doc.DocumentElement);

            NormalGraphBuilder builder = new NormalGraphBuilder();
            parser.ParseProfilesAndMachines(builder);

            _defaults = builder.DefaultManager;
        }


        [Test]
        public void GotTheDefaultProfileName()
        {
            Assert.AreEqual("Blue", _defaults.DefaultProfileName);
        }

        [Test]
        public void GotProfileOnGreenBox()
        {
            MachineOverride greenBox = _defaults.GetMachineOverride("GREEN-BOX");
            Assert.AreEqual("Green", greenBox.ProfileName);
        }

        [Test]
        public void GotDefaultsOnBlueProfile()
        {
            InstanceDefault[] defaults = new InstanceDefault[]
                {
                    new InstanceDefault("StructureMap.Testing.Widget.IWidget", "Blue"),
                    new InstanceDefault("StructureMap.Testing.Widget.Rule", "Blue"),
                    new InstanceDefault("StructureMap.Testing.Widget4.IStrategy", "Blue")
                };

            // You're my boy, Blue!
            Profile blue = _defaults.GetProfile("Blue");
            Assert.AreEqual(defaults, blue.Defaults);
        }

        [Test]
        public void GotDefaultsOnServerMachine()
        {
            InstanceDefault[] defaults = new InstanceDefault[]
                {
                    new InstanceDefault("StructureMap.Testing.Widget.IWidget", "Orange"),
                    new InstanceDefault("StructureMap.Testing.Widget4.IStrategy", "Blue")
                };

            MachineOverride machine = _defaults.GetMachineOverride("SERVER");
            Assert.AreEqual(defaults, machine.InnerDefaults);
        }

        [Test]
        public void GotAllProfiles()
        {
            Assert.AreEqual(new string[] {"Blue", "Green"}, _defaults.GetProfileNames());
        }

        [Test]
        public void GotAllMachines()
        {
            string[] names = _defaults.GetMachineNames();
            Assert.AreEqual(new string[] {"SERVER", "GREEN-BOX"}, names);
        }
    }
}