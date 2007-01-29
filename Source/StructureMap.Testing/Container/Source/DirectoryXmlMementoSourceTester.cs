using System;
using System.IO;
using NUnit.Framework;
using StructureMap.Source;

namespace StructureMap.Testing.Container.Source
{
    [TestFixture]
    public class DirectoryXmlMementoSourceTester
    {
        private DirectoryXmlMementoSource _source;


        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Directory.CreateDirectory("MementoDirectory");

            string instance1 =
                "<Instance Key=\"Red\" Type=\"Color\"><Property Name=\"Color\" Value=\"Red\" /></Instance>";
            string instance2 =
                "<Instance Key=\"Blue\" Type=\"Color\"><Property Name=\"Color\" Value=\"Blue\" /></Instance>";
            string instance3 =
                "<Instance Key=\"Bigger\" Type=\"GreaterThan\"><Property Name=\"Attribute\" Value=\"MyDad\" /><Property Name=\"Value\" Value=\"10\" /></Instance>";

            writeFile(instance1, @"MementoDirectory\Red.xml");
            writeFile(instance2, @"MementoDirectory\Blue.xml");
            writeFile(instance3, @"MementoDirectory\Bigger.xml");

            _source = new DirectoryXmlMementoSource("MementoDirectory", "xml", XmlMementoStyle.NodeNormalized);
        }

        private void writeFile(string text, string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            StreamWriter writer = fileInfo.CreateText();
            writer.Write(text);

            writer.Close();
        }

        [Test]
        public void CanGetAllMementos()
        {
            InstanceMemento[] mementos = _source.GetAllMementos();
            Assert.IsNotNull(mementos);
            Assert.AreEqual(3, mementos.Length);
        }

        [Test]
        public void GetRedInstance()
        {
            InstanceMemento memento = _source.GetMemento("Red");
            Assert.IsNotNull(memento);
            Assert.AreEqual("Red", memento.GetProperty("Color"));
        }

        [Test]
        public void ValidateIsSuccessfulWithAnExistingDirectory()
        {
            _source.Validate();
        }

        [Test, ExpectedException(typeof (ApplicationException))]
        public void ValidateIsNotSuccessfulWithADirectoryItCannotFind()
        {
            DirectoryXmlMementoSource source =
                new DirectoryXmlMementoSource("NotARealDirectory", "xml", XmlMementoStyle.NodeNormalized);
            source.Validate();
        }
    }
}