using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess.Tools;
using StructureMap.DataAccess.Tools.Mocks;

namespace StructureMap.Testing.DataAccess.Tools.Mocks
{
    [TestFixture]
    public class MockReaderSourceTester
    {
        [Test, ExpectedException(typeof (ParameterValidationFailureException))]
        public void ExecuteOnceOkaySecondTimeWithBadParameters()
        {
            TableDataReader result1 = new TableDataReader();
            TableDataReader result2 = new TableDataReader();

            ReaderExpectation expectation1 = new ReaderExpectation(new ParameterList(), result1);
            ReaderExpectation expectation2 = new ReaderExpectation(new ParameterList(), result2);

            MockReaderSource source = new MockReaderSource("name");
            source.AddExpectation(expectation1);
            source.AddExpectation(expectation2);

            source.ExecuteReader();
            source["nonsense"] = 0;
            source.ExecuteReader();
        }

        [Test]
        public void ExecuteOnceWithExpectationAllSuccess()
        {
            MockReaderSource source = new MockReaderSource("name");
            TableDataReader result = new TableDataReader();
            ReaderExpectation expectation = new ReaderExpectation(new ParameterList(), result);
            source.AddExpectation(expectation);

            IDataReader reader = source.ExecuteReader();

            Assert.AreSame(result, reader);
        }

        [Test, ExpectedException(typeof (ParameterValidationFailureException))]
        public void ExecuteOnceWithInvalidParameters()
        {
            MockReaderSource source = new MockReaderSource("name");
            TableDataReader result = new TableDataReader();
            ReaderExpectation expectation = new ReaderExpectation(new ParameterList(), result);
            source.AddExpectation(expectation);

            source["UnknownParameter"] = "okay";

            IDataReader reader = source.ExecuteReader();
        }

        [Test]
        public void ExecuteThreeTimesNoErrors()
        {
            TableDataReader result1 = new TableDataReader();
            TableDataReader result2 = new TableDataReader();
            TableDataReader result3 = new TableDataReader();

            ReaderExpectation expectation1 = new ReaderExpectation(new ParameterList(), result1);
            ReaderExpectation expectation2 = new ReaderExpectation(new ParameterList(), result2);
            ReaderExpectation expectation3 = new ReaderExpectation(new ParameterList(), result3);

            MockReaderSource source = new MockReaderSource("name");
            source.AddExpectation(expectation1);
            source.AddExpectation(expectation2);
            source.AddExpectation(expectation3);

            Assert.AreSame(result1, source.ExecuteReader());
            Assert.AreSame(result2, source.ExecuteReader());
            Assert.AreSame(result3, source.ExecuteReader());
        }


        [Test, ExpectedException(typeof (UnExpectedCallException))]
        public void ExecuteThreeTimesWithTwoExpectationsFailOnThird()
        {
            TableDataReader result1 = new TableDataReader();
            TableDataReader result2 = new TableDataReader();

            ReaderExpectation expectation1 = new ReaderExpectation(new ParameterList(), result1);
            ReaderExpectation expectation2 = new ReaderExpectation(new ParameterList(), result2);

            MockReaderSource source = new MockReaderSource("name");
            source.AddExpectation(expectation1);
            source.AddExpectation(expectation2);

            Assert.AreSame(result1, source.ExecuteReader());
            Assert.AreSame(result2, source.ExecuteReader());

            // Should blow up on third try
            source.ExecuteReader();
        }

        [Test, ExpectedException(typeof (UnExpectedCallException))]
        public void FailIfCalledTooManyTimes()
        {
            MockReaderSource source = new MockReaderSource("name");
            source.ExecuteReader();
        }
    }
}