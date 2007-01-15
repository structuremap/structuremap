using NUnit.Framework;
using StructureMap.DataAccess.Tools.Mocks;

namespace StructureMap.Testing.DataAccess.Tools.Mocks
{
	[TestFixture]
	public class MockCommandTester
	{
		[Test, ExpectedException(typeof(UnExpectedCallException))]
		public void ExecuteWithoutExpectation()
		{
			MockCommand command = new MockCommand("name");
			command.Execute();
		}


		[Test]
		public void ExecuteWithExpectationNoParameters()
		{
			MockCommand command = new MockCommand("name");

			CommandExpectation expectation = new CommandExpectation(5);
			command.AddExpectation(expectation);

			int count = command.Execute();

			Assert.AreEqual(5, count);
		}


		[Test]
		public void ExecuteWithInputParametersThatMatchExpectation()
		{
			MockCommand command = new MockCommand("name");
			command["param1"] = "Bo";
			command["param2"] = "Luke";
			command["param3"] = "Daisy";

			CommandExpectation expectation = new CommandExpectation(5);
			expectation.SetInput("param1", "Bo");
			expectation.SetInput("param2", "Luke");
			expectation.SetInput("param3", "Daisy");
			command.AddExpectation(expectation);

			int count = command.Execute();

			Assert.AreEqual(5, count);
		}

		[Test, ExpectedException(typeof(ParameterValidationFailureException))]
		public void ExecuteWithInputParametersThatDoNotMatchExpectation()
		{
			MockCommand command = new MockCommand("name");
			command["param1"] = "Bo";
			command["param2"] = "Luke";
			command["param3"] = "Daisy";

			CommandExpectation expectation = new CommandExpectation(5);
			expectation.SetInput("param1", "Bo");
			expectation.SetInput("param2", "Luke");
			expectation.SetInput("param3", "Boss Hogg");
			command.AddExpectation(expectation);

			int count = command.Execute();
		}

		[Test]
		public void ExecuteAndGetOutputParameters()
		{
			MockCommand command = new MockCommand("name");

			CommandExpectation expectation = new CommandExpectation(5);
			expectation.SetOutput("param1", "Bo");
			command.AddExpectation(expectation);

			int count = command.Execute();

			Assert.AreEqual("Bo", command["param1"]);
		}

		[Test, ExpectedException(typeof(NotExecutedCommandException))]
		public void TryToGetOutputParametersBeforeExecute()
		{
			MockCommand command = new MockCommand("name");

			CommandExpectation expectation = new CommandExpectation(5);
			expectation.SetOutput("param1", "Bo");
			command.AddExpectation(expectation);

			string output = (string) command["param1"];
		}

		[Test, ExpectedException(typeof(UnKnownOrNotSetParameterException))]
		public void TryToGetInputParameterThatHasNotBeenSet()
		{
			MockCommand command = new MockCommand("name");
			object answer = command["param1"];
		}
	
		[Test]
		public void TryToGetInputParameterThatHasBeenSet()
		{
			MockCommand command = new MockCommand("name");
			command["param1"] = true;
			
			Assert.IsTrue((bool) command["param1"]);
		}

		[Test, ExpectedException(typeof(UnExpectedCallException))]
		public void ExecuteTooManyTimes()
		{
			MockCommand command = new MockCommand("name");
			command.AddExpectation(new CommandExpectation(3));
			command.AddExpectation(new CommandExpectation(3));

			command.Execute();
			command.Execute();
			command.Execute();
		}

		[Test]
		public void ExecuteTwiceAndGetResultsBothTimes()
		{
			MockCommand command = new MockCommand("name");

			CommandExpectation expectation1 = new CommandExpectation();
			expectation1.SetOutput("answer", "red");
			CommandExpectation expectation2 = new CommandExpectation();
			expectation2.SetOutput("answer", "green");

			command.AddExpectation(expectation1);
			command.AddExpectation(expectation2);

			command.Execute();

			Assert.AreEqual("red", command["answer"]);

			command.Execute();
			Assert.AreEqual("green", command["answer"]);
		}

	}
}
