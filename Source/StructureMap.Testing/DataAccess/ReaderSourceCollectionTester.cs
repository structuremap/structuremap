using System.Data;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess
{
    // TODO -- replace with Rhino
    [TestFixture]
    public class ReaderSourceCollectionTester
    {
        //[Test]
        //public void GetAReaderSourceThreeTimesGetTheSameCommand()
        //{
        //    IMock factoryMock = new DynamicMock(typeof (ICommandFactory));
        //    string theReaderSourceName = "name of command";

        //    StubbedReaderSource stubbedReaderSource = new StubbedReaderSource(new TableDataReader(new DataTable()));

        //    factoryMock.ExpectAndReturn("BuildReaderSource", stubbedReaderSource, theReaderSourceName);

        //    DataSession session = ObjectMother.MSSQLDataSession();
        //    ReaderSourceCollection readerSources =
        //        new ReaderSourceCollection(session, (ICommandFactory) factoryMock.MockInstance);

        //    Assert.AreSame(stubbedReaderSource, readerSources[theReaderSourceName]);
        //    Assert.AreSame(stubbedReaderSource, readerSources[theReaderSourceName]);
        //    Assert.AreSame(stubbedReaderSource, readerSources[theReaderSourceName]);

        //    Assert.AreSame(session, stubbedReaderSource.Session);
        //}
    }
}