using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.Tools;
using StructureMap.DataAccess.Tools.Mocks;

namespace StructureMap.Testing.DataAccess.Tools.Mocks
{
    [TestFixture]
    public class MockDataSessionTester
    {
        [Test]
        public void ExecuteADataReaderSource()
        {
            var session = new MockDataSession();

            var result = new TableDataReader();
            var expectation = new ReaderExpectation(new ParameterList(), result);

            string theReaderSourceName = "reader source";
            session.AddReaderExpectation(theReaderSourceName, expectation);

            IReaderSource source = session.ReaderSources[theReaderSourceName];
            IDataReader reader = source.ExecuteReader();

            Assert.AreSame(result, reader);
        }

        [Test]
        public void ExecuteAMockCommand()
        {
            var session = new MockDataSession();

            var expectation = new CommandExpectation(4);
            expectation.SetInput("input", "me");
            expectation.SetOutput("output", "you");

            session.AddCommandExpectation("MeToYou", expectation);

            ICommand command = session.Commands["MeToYou"];

            command["input"] = "me";
            int count = command.Execute();

            Assert.AreEqual(4, count);
            Assert.AreEqual("you", command["output"]);
        }
    }
}