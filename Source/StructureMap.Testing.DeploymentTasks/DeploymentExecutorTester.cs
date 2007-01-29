using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using StructureMap.DeploymentTasks;
using StructureMap.Testing.XmlWriting;

namespace StructureMap.Testing.DeploymentTasks
{
    [TestFixture]
    public class DeploymentExecutorTester
    {
        private string _deploymentTarget;
        private MachineSpecificOption _machineOption;
        private string _machineName;
        private string _profile;
        private string _expectedXmlPath;

        [SetUp]
        public void SetUp()
        {
            _deploymentTarget = string.Empty;
            _machineOption = MachineSpecificOption.CopyMachineOverrides;
            _machineName = string.Empty;
            _profile = string.Empty;
            _expectedXmlPath = string.Empty;
        }


        private void runDeploymentTest()
        {
            XmlDocument expectedDoc = new XmlDocument();
            expectedDoc.Load(_expectedXmlPath);
            ElementChecker checker = new ElementChecker(expectedDoc.DocumentElement);

            DeploymentExecutor executor = new DeploymentExecutor("DeploymentSourceConfig.xml", "result.xml",
                                                                 _deploymentTarget, _profile, _machineName,
                                                                 _machineOption);

            XmlDocument actualDoc = executor.BuildConfigDocument();
            string outputDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            actualDoc.Save(Path.Combine(outputDirectory, @"Results.xml"));
            checker.Check(actualDoc.DocumentElement);
        }


        [Test]
        public void StraightCopy()
        {
            _expectedXmlPath = "DeploymentSourceConfig.xml";
            runDeploymentTest();
        }

        [Test]
        public void WithProfileAndNothingElseCopyMachineOverrides()
        {
            _expectedXmlPath = "WithProfileAndNothingElseCopyMachineOverrides.xml";
            _profile = "Blue";
            _machineOption = MachineSpecificOption.CopyMachineOverrides;
            runDeploymentTest();
        }


        [Test]
        public void WithProfileAndNothingElseIgnoreMachineOverrides()
        {
            _expectedXmlPath = "WithProfileAndNothingElseIgnoreMachineOverrides.xml";
            _profile = "Blue";
            _machineOption = MachineSpecificOption.IgnoreMachineOverrides;
            runDeploymentTest();
        }


        [Test]
        public void WithProfileAndUseCurrentMachineOverride()
        {
            _machineName = "SERVER";
            _expectedXmlPath = "WithProfileAndUseCurrentMachineOverride.xml";
            _profile = "Blue";
            _machineOption = MachineSpecificOption.UseCurrentMachineOverride;
            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsClientCopyMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsClientCopyMachineOverrides.xml";
            _deploymentTarget = "Client";
            _machineOption = MachineSpecificOption.CopyMachineOverrides;
            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsClientIgnoreMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsClientIgnoreMachineOverrides.xml";
            _deploymentTarget = "Client";
            _machineOption = MachineSpecificOption.IgnoreMachineOverrides;
            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsClientUseCurrentMachineOverride()
        {
            _expectedXmlPath = "DeploymentTargetIsClientUseCurrentMachineOverride.xml";
            _deploymentTarget = "Client";
            _machineOption = MachineSpecificOption.UseCurrentMachineOverride;
            _machineName = "SERVER";
            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsServerCopyMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsServerCopyMachineOverrides.xml";
            _deploymentTarget = "Server";
            _machineOption = MachineSpecificOption.CopyMachineOverrides;

            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsServerUseCurrentMachineOverride()
        {
            _expectedXmlPath = "DeploymentTargetIsServerUseCurrentMachineOverride.xml";
            _deploymentTarget = "Server";
            _machineOption = MachineSpecificOption.UseCurrentMachineOverride;
            _machineName = "SERVER";

            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsServerIgnoreMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsServerIgnoreMachineOverrides.xml";
            _deploymentTarget = "Server";
            _machineOption = MachineSpecificOption.IgnoreMachineOverrides;


            runDeploymentTest();
        }

        [Test]
        public void DeploymentTargetIsAcceptanceTesting()
        {
            _expectedXmlPath = "DeploymentTargetIsAcceptanceTesting.xml";
            _deploymentTarget = "AcceptanceTesting";
            _machineOption = MachineSpecificOption.CopyMachineOverrides;

            runDeploymentTest();
        }

        [Test]
        public void MachineNameReferencesProfileUseCurrentMachineOverride()
        {
            _expectedXmlPath = "MachineNameReferencesProfileUseCurrentMachineOverride.xml";
            _machineName = "GREENBOX";
            _machineOption = MachineSpecificOption.UseCurrentMachineOverride;

            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsServerProfileIsGreenIgnoreMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsServerProfileIsGreenIgnoreMachineOverrides.xml";
            _deploymentTarget = "Server";
            _profile = "Green";
            _machineOption = MachineSpecificOption.IgnoreMachineOverrides;

            runDeploymentTest();
        }

        [Test]
        public void DeploymentTargetIsServerProfileIsGreenCopyMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsServerProfileIsGreenCopyMachineOverrides.xml";
            _deploymentTarget = "Server";
            _profile = "Green";
            _machineOption = MachineSpecificOption.CopyMachineOverrides;

            runDeploymentTest();
        }

        [Test]
        public void DeploymentTargetIsServerProfileIsGreenUseCurrentMachineOverride()
        {
            _expectedXmlPath = "DeploymentTargetIsServerProfileIsGreenUseCurrentMachineOverride.xml";
            _deploymentTarget = "Server";
            _profile = "Green";
            _machineOption = MachineSpecificOption.UseCurrentMachineOverride;

            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsClientProfileIsBlueIgnoreMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsClientProfileIsBlueIgnoreMachineOverrides.xml";
            _deploymentTarget = "Client";
            _profile = "Blue";
            _machineOption = MachineSpecificOption.IgnoreMachineOverrides;

            runDeploymentTest();
        }


        [Test]
        public void DeploymentTargetIsClientProfileIsBlueCopyMachineOverrides()
        {
            _expectedXmlPath = "DeploymentTargetIsClientProfileIsBlueCopyMachineOverrides.xml";
            _deploymentTarget = "Client";
            _profile = "Blue";
            _machineOption = MachineSpecificOption.CopyMachineOverrides;

            runDeploymentTest();
        }
    }
}