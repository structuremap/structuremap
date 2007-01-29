using System.Xml;
using NUnit.Framework;
using StructureMap.DeploymentTasks;

namespace StructureMap.Testing.DeploymentTasks
{
    [TestFixture]
    public class SubstitutionTaskTester
    {
        [SetUp]
        public void SetUp()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml("<StructureMap DefaultProfile=\"{profile}\" />");
            document.Save("Substitution.xml");
        }

        [Test]
        public void ReplaceTextInXml()
        {
            SubstitutionTask task = new SubstitutionTask();

            task.Flag = "profile";
            task.FilePath = "Substitution.xml";
            task.Value = "Blue";
            task.DoWork();

            XmlDocument document = new XmlDocument();
            document.Load("Substitution.xml");
            Assert.AreEqual("Blue", document.DocumentElement.GetAttribute("DefaultProfile"));
        }
    }
}