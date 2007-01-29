using System.Xml;
using NUnit.Framework;
using StructureMap.Testing.XmlWriting;

namespace StructureMap.Testing.DeploymentTasks
{
    /// <summary>
    /// This TestFixture only serves to test the output of the custom structuremap.deployment
    /// NAnt step in the StructureMap.build file
    /// 
    /// It will not pass except from with the NAnt build
    /// </summary>
    [TestFixture, Explicit] //Need to see where the "actual" files are.
    public class DeploymentTaskAcceptanceTests
    {
        private void checkFileAgainstExpectation(string fileName)
        {
            XmlDocument expectedDoc = new XmlDocument();
            expectedDoc.Load(fileName);

            XmlDocument actualDoc = new XmlDocument();
            actualDoc.Load(fileName + ".actual");

            ElementChecker checker = new ElementChecker(expectedDoc.DocumentElement);

            checker.Check(actualDoc.DocumentElement);
        }

        [Test]
        public void WithProfileAndNothingElseCopyMachineOverrides()
        {
            checkFileAgainstExpectation("WithProfileAndNothingElseCopyMachineOverrides.xml");
        }


        [Test]
        public void WithProfileAndNothingElseIgnoreMachineOverrides()
        {
            checkFileAgainstExpectation("WithProfileAndNothingElseIgnoreMachineOverrides.xml");
        }

        [Test]
        public void DeploymentTargetIsClientCopyMachineOverrides()
        {
            checkFileAgainstExpectation("DeploymentTargetIsClientCopyMachineOverrides.xml");
        }
    }
}