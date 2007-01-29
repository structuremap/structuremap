using NUnit.Framework;
using StructureMap.DataAccess.Tools.Mocks;

namespace StructureMap.Testing.DataAccess.Tools.Mocks
{
    [TestFixture]
    public class CommandExpectationTester
    {
        [Test]
        public void SetAndRetrieveInputs()
        {
            CommandExpectation expectation = new CommandExpectation(1);
            expectation.SetInput("param1", 1);

            Assert.AreEqual(1, (int) expectation.GetInput("param1"));
        }

        [Test]
        public void SetAndRetrieveOutputsAfterExecution()
        {
            CommandExpectation expectation = new CommandExpectation(1);
            expectation.SetOutput("param1", 1);

            expectation.VerifyExecution(new ParameterList());

            Assert.AreEqual(1, (int) expectation.GetOutput("param1"));
        }

        [Test, ExpectedException(typeof (NotExecutedCommandException))]
        public void TryToGetOutputBeforeExecutionAndFail()
        {
            CommandExpectation expectation = new CommandExpectation(1);
            expectation.SetOutput("param1", 1);

            expectation.GetOutput("param1");
        }

        [Test]
        public void IsOutput()
        {
            CommandExpectation expectation = new CommandExpectation(1);
            expectation.SetOutput("param1", 1);

            Assert.IsTrue(expectation.IsOutput("param1"));
            Assert.IsFalse(expectation.IsOutput("param2"));
        }

        [Test]
        public void VerifyAllIsGoodAndGetOutput()
        {
            CommandExpectation expectation = new CommandExpectation(1);
            expectation.SetInput("input", true);
            expectation.SetOutput("output", true);

            ParameterList list = new ParameterList();
            list["input"] = true;

            expectation.VerifyExecution(list);

            Assert.IsTrue((bool) expectation.GetOutput("output"));
        }

        [Test, ExpectedException(typeof (ParameterValidationFailureException))]
        public void VerifyExecutionCalledWithIncorrectParameters()
        {
            CommandExpectation expectation = new CommandExpectation();
            expectation.SetInput("input", true);
            expectation.SetOutput("output", true);

            ParameterList list = new ParameterList();
            list["input"] = false;

            expectation.VerifyExecution(list);
        }
    }
}