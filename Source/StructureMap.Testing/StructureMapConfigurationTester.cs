using System;
using System.IO;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Testing
{
    [TestFixture]
    public class StructureMapConfigurationTester
    {
        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
        }

        [Test]
        public void BuildPluginGraph()
        {
            PluginGraph graph = StructureMapConfiguration.GetPluginGraph();
            Assert.IsNotNull(graph);
        }

        [Test]
        public void BuildReport()
        {
            PluginGraphReport report = StructureMapConfiguration.GetDiagnosticReport();
            Assert.IsNotNull(report);
        }

        [Test, ExpectedException(typeof(ApplicationException), "StructureMap detected configuration or environmental problems.  Check the StructureMap error file")]
        public void OnStartUpFail()
        {
            StructureMapConfiguration.OnStartUp().FailOnException();
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();
        }

        [Test]
        public void WriteAllFile()
        {
            string filePath = "all.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            StructureMapConfiguration.OnStartUp().WriteAllTo(filePath);
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();

            Assert.IsTrue(File.Exists(filePath));
        }


        [Test]
        public void WriteProblems()
        {
            string filePath = "problems.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            StructureMapConfiguration.OnStartUp().WriteProblemsTo(filePath);
            StructureMapConfiguration.AddInstanceOf<ISomething>().UsingConcreteType<Something>();

            StructureMapConfiguration.GetPluginGraph();

            Assert.IsTrue(File.Exists(filePath));
        }
    }

    public interface ISomething
    {
        
    }

    public class Something : ISomething
    {
        public Something()
        {
            throw new ApplicationException("You can't make me!");
        }
    }
}