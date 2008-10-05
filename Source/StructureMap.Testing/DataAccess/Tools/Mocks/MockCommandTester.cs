using NUnit.Framework;
using StructureMap.DataAccess.Tools.Mocks;

namespace StructureMap.Testing.DataAccess.Tools.Mocks
{
    [TestFixture]
    public class MockCommandTester
    {
        [Test]
        public void ExecuteAndGetOutputParameters()
        {
            var command = new MockCommand("name");

            var expectation = new CommandExpectation(5);
            expectation.SetOutput("param1", "Bo");
            command.AddExpectation(expectation);

            int count = command.Execute();

            Assert.AreEqual("Bo", command["param1"]);
        }

        [Test, ExpectedException(typeof (UnExpectedCallException))]
        public void ExecuteTooManyTimes()
        {
            var command = new MockCommand("name");
            command.AddExpectation(new CommandExpectation(3));
            command.AddExpectation(new CommandExpectation(3));

            command.Execute();
            command.Execute();
            command.Execute();
        }

        [Test]
        public void ExecuteTwiceAndGetResultsBothTimes()
        {
            var command = new MockCommand("name");

            var expectation1 = new CommandExpectation();
            expectation1.SetOutput("answer", "red");
            var expectation2 = new CommandExpectation();
            expectation2.SetOutput("answer", "green");

            command.AddExpectation(expectation1);
            command.AddExpectation(expectation2);

            command.Execute();

            Assert.AreEqual("red", command["answer"]);

            command.Execute();
            Assert.AreEqual("green", command["answer"]);
        }

        [Test]
        public void ExecuteWithExpectationNoParameters()
        {
            var command = new MockCommand("name");

            var expectation = new CommandExpectation(5);
            command.AddExpectation(expectation);

            int count = command.Execute();

            Assert.AreEqual(5, count);
        }

        [Test, ExpectedException(typeof (ParameterValidationFailureException))]
        public void ExecuteWithInputParametersThatDoNotMatchExpectation()
        {
            var command = new MockCommand("name");
            command["param1"] = "Bo";
            command["param2"] = "Luke";
            command["param3"] = "Daisy";

            var expectation = new CommandExpectation(5);
            expectation.SetInput("param1", "Bo");
            expectation.SetInput("param2", "Luke");
            expectation.SetInput("param3", "Boss Hogg");
            command.AddExpectation(expectation);

            int count = command.Execute();
        }

        [Test]
        public void ExecuteWithInputParametersThatMatchExpectation()
        {
            var command = new MockCommand("name");
            command["param1"] = "Bo";
            command["param2"] = "Luke";
            command["param3"] = "Daisy";

            var expectation = new CommandExpectation(5);
            expectation.SetInput("param1", "Bo");
            expectation.SetInput("param2", "Luke");
            expectation.SetInput("param3", "Daisy");
            command.AddExpectation(expectation);

            int count = command.Execute();

            Assert.AreEqual(5, count);
        }

        [Test, ExpectedException(typeof (UnExpectedCallException))]
        public void ExecuteWithoutExpectation()
        {
            var command = new MockCommand("name");
            command.Execute();
        }

        [Test]
        public void TryToGetInputParameterThatHasBeenSet()
        {
            var command = new MockCommand("name");
            command["param1"] = true;

            Assert.IsTrue((bool) command["param1"]);
        }

        [Test, ExpectedException(typeof (UnKnownOrNotSetParameterException))]
        public void TryToGetInputParameterThatHasNotBeenSet()
        {
            var command = new MockCommand("name");
            object answer = command["param1"];
        }

        [Test, ExpectedException(typeof (NotExecutedCommandException))]
        public void TryToGetOutputParametersBeforeExecute()
        {
            var command = new MockCommand("name");

            var expectation = new CommandExpectation(5);
            expectation.SetOutput("param1", "Bo");
            command.AddExpectation(expectation);

            var output = (string) command["param1"];
        }
    }
}