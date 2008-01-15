using NUnit.Framework;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class UserControlExpressionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void CreateMementoHappyPath()
        {
            string theUrl = "alskdjf";

            UserControlExpression expression = new UserControlExpression(typeof (IControl), theUrl);

            UserControlMemento memento =
                (UserControlMemento) ((IMementoBuilder) expression).BuildMemento(new PluginGraph());
            Assert.IsNotNull(memento);

            Assert.AreEqual(theUrl, memento.Url);
            Assert.IsNotEmpty(memento.InstanceKey);
            Assert.IsNotNull(memento.InstanceKey);
        }

        [Test]
        public void CreateMementoHappyPathWithName()
        {
            string theUrl = "alskdjf";
            string theName = "the name";

            UserControlExpression expression = new UserControlExpression(typeof (IControl), theUrl);
            expression.WithName(theName);

            UserControlMemento memento =
                (UserControlMemento) ((IMementoBuilder) expression).BuildMemento(new PluginGraph());
            Assert.IsNotNull(memento);

            Assert.AreEqual(theUrl, memento.Url);
            Assert.AreEqual(theName, memento.InstanceKey);
        }
    }

    public interface IControl
    {
    }
}