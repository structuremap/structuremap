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
            var result1 = new TableDataReader();
            var result2 = new TableDataReader();

            var expectation1 = new ReaderExpectation(new ParameterList(), result1);
            var expectation2 = new ReaderExpectation(new ParameterList(), result2);

            var source = new MockReaderSource("name");
            source.AddExpectation(expectation1);
            source.AddExpectation(expectation2);

            source.ExecuteReader();
            source["nonsense"] = 0;
            source.ExecuteReader();
        }

        [Test]
        public void ExecuteOnceWithExpectationAllSuccess()
        {
            var source = new MockReaderSource("name");
            var result = new TableDataReader();
            var expectation = new ReaderExpectation(new ParameterList(), result);
            source.AddExpectation(expectation);

            IDataReader reader = source.ExecuteReader();

            Assert.AreSame(result, reader);
        }

        [Test, ExpectedException(typeof (ParameterValidationFailureException))]
        public void ExecuteOnceWithInvalidParameters()
        {
            var source = new MockReaderSource("name");
            var result = new TableDataReader();
            var expectation = new ReaderExpectation(new ParameterList(), result);
            source.AddExpectation(expectation);

            source["UnknownParameter"] = "okay";

            IDataReader reader = source.ExecuteReader();
        }

        [Test]
        public void ExecuteThreeTimesNoErrors()
        {
            var result1 = new TableDataReader();
            var result2 = new TableDataReader();
            var result3 = new TableDataReader();

            var expectation1 = new ReaderExpectation(new ParameterList(), result1);
            var expectation2 = new ReaderExpectation(new ParameterList(), result2);
            var expectation3 = new ReaderExpectation(new ParameterList(), result3);

            var source = new MockReaderSource("name");
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
            var result1 = new TableDataReader();
            var result2 = new TableDataReader();

            var expectation1 = new ReaderExpectation(new ParameterList(), result1);
            var expectation2 = new ReaderExpectation(new ParameterList(), result2);

            var source = new MockReaderSource("name");
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
            var source = new MockReaderSource("name");
            source.ExecuteReader();
        }
    }
}