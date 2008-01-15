using NUnit.Framework;
using StructureMap.Configuration.Tokens;
using StructureMap.Source;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Container.Source
{
    [TestFixture]
    public class TemplatingTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.WriteDocument(FILE_NAME);

            _nodeTemplateSource = new XmlFileMementoSource(FILE_NAME, "NodeTemplates", "Parent");
            _attTemplateSource = new XmlAttributeFileMementoSource(FILE_NAME, "AttTemplates", "Parent");
            _source = new XmlAttributeFileMementoSource(FILE_NAME, "Parents", "Parent");

            _referringMemento = _source.GetMemento("Jackie");

            _templatedSource = new TemplatedMementoSource(_source, _attTemplateSource);
        }

        #endregion

        private const string FILE_NAME = "InstanceMementoTemplating.xml";
        private MementoSource _nodeTemplateSource;
        private MementoSource _attTemplateSource;
        private MementoSource _source;
        private InstanceMemento _referringMemento;
        private TemplatedMementoSource _templatedSource;

        private void validateCombinedMemento(InstanceMemento combinedMemento)
        {
            Assert.AreEqual("70", combinedMemento.GetProperty("Age"));
            Assert.AreEqual("Green", combinedMemento.GetProperty("EyeColor"));

            InstanceMemento grandChildMemeto =
                combinedMemento.GetChildMemento("MyChild").GetChildMemento("MyGrandChild");
            Assert.AreEqual("1992", grandChildMemeto.GetProperty("BirthYear"));
        }


        [Test]
        public void GetAllMementos()
        {
            InstanceMemento[] mementos = _templatedSource.GetAllMementos();

            Assert.IsNotNull(mementos);
            Assert.AreEqual(4, mementos.Length);

            foreach (InstanceMemento memento in mementos)
            {
                Assert.IsNotNull(memento);
            }
        }

        [Test]
        public void GetAllTemplates()
        {
            TemplateToken[] tokens = _templatedSource.GetAllTemplates();
            Assert.AreEqual(1, tokens.Length);

            TemplateToken template = tokens[0];
            Assert.AreEqual("Default", template.ConcreteKey);
            Assert.AreEqual(3, template.Properties.Length);
            Assert.AreEqual("Grandmother", template.TemplateKey);
        }

        [Test]
        public void GetNonTemplatedInstance()
        {
            InstanceMemento memento = _templatedSource.GetMemento("Nadine");
            Assert.AreEqual("80", memento.GetProperty("Age"));
            Assert.AreEqual("Blue", memento.GetProperty("EyeColor"));
        }

        [Test]
        public void GetTemplatedMementoFromAttributeNormalizedTemplate()
        {
            InstanceMemento templateMemento = _attTemplateSource.GetMemento("Grandmother");
            Assert.IsNotNull(templateMemento);
            InstanceMemento combinedMemento = templateMemento.Substitute(_referringMemento);

            validateCombinedMemento(combinedMemento);
        }

        [Test]
        public void GetTemplatedMementoFromNodeNormalizedTemplate()
        {
            InstanceMemento templateMemento = _nodeTemplateSource.GetMemento("Grandmother");
            Assert.IsNotNull(templateMemento);
            InstanceMemento combinedMemento = templateMemento.Substitute(_referringMemento);

            validateCombinedMemento(combinedMemento);
        }

        [Test]
        public void GetTheTemplatedInstance()
        {
            InstanceMemento memento = _templatedSource.GetMemento("Jackie");
            validateCombinedMemento(memento);
        }
    }
}