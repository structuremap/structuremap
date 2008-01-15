using System.Xml;
using NUnit.Framework;
using StructureMap.DeploymentTasks;

namespace StructureMap.Testing.DeploymentTasks
{
    [TestFixture]
    public class SetOverrideTaskTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _document = new XmlDocument();
            _document.LoadXml(configXml);
        }

        #endregion

        private string configXml = "<StructureMap DefaultProfile=\"Blue\">" +
                                   "<Profile Name=\"Blue\"><Override Type=\"something\" DefaultKey=\"BlueKey\"/></Profile>" +
                                   "<Profile Name=\"Red\"/>" +
                                   "<Profile Name=\"Green\"><Override Type=\"something\" DefaultKey=\"GreenKey\"/></Profile>" +
                                   "</StructureMap>";

        private XmlDocument _document;


        [Test]
        public void SetAnOverrideOnANamedProfile()
        {
            SetOverrideTask task = new SetOverrideTask();
            task.TypeName = "something";
            string theNewKey = "newKey";
            task.DefaultKey = theNewKey;
            task.ProfileName = "Green";

            task.ApplyToDocument(_document);

            XmlNode overrideNode =
                _document.DocumentElement.SelectSingleNode("Profile[@Name='Green']/Override[@Type='something']");
            string actualKey = overrideNode.Attributes["DefaultKey"].InnerText;

            Assert.AreEqual(theNewKey, actualKey);
        }

        [Test]
        public void SetAnOverrideOnANamedProfileIfTheOverrideNodeDoesNotExist()
        {
            SetOverrideTask task = new SetOverrideTask();
            task.TypeName = "something";
            string theNewKey = "newKey";
            task.DefaultKey = theNewKey;
            task.ProfileName = "Red";

            task.ApplyToDocument(_document);

            XmlNode overrideNode =
                _document.DocumentElement.SelectSingleNode("Profile[@Name='Red']/Override[@Type='something']");
            string actualKey = overrideNode.Attributes["DefaultKey"].InnerText;

            Assert.AreEqual(theNewKey, actualKey);
        }


        [Test]
        public void SetAnOverrideOnANamedProfileIfTheProfileNodeDoesNotExist()
        {
            SetOverrideTask task = new SetOverrideTask();
            task.TypeName = "something";
            string theNewKey = "newKey";
            task.DefaultKey = theNewKey;
            task.ProfileName = "Orange";

            task.ApplyToDocument(_document);

            XmlNode overrideNode =
                _document.DocumentElement.SelectSingleNode("Profile[@Name='Orange']/Override[@Type='something']");
            string actualKey = overrideNode.Attributes["DefaultKey"].InnerText;

            Assert.AreEqual(theNewKey, actualKey);
        }

        [Test]
        public void SetAnOverrideOnTheDefaultProfile()
        {
            SetOverrideTask task = new SetOverrideTask();
            task.TypeName = "something";
            string theNewKey = "newKey";
            task.DefaultKey = theNewKey;

            task.ApplyToDocument(_document);

            XmlNode overrideNode =
                _document.DocumentElement.SelectSingleNode("Profile[@Name='Blue']/Override[@Type='something']");
            string actualKey = overrideNode.Attributes["DefaultKey"].InnerText;

            Assert.AreEqual(theNewKey, actualKey);
        }
    }
}