using System.Data;
using NMock;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.Tools;

namespace StructureMap.Testing.DataAccess
{
    [TestFixture, Explicit]
    public class CommandFactoryTester
    {
        #region Setup/Teardown

        [TearDown]
        public void TearDown()
        {
            ObjectFactory.ResetDefaults();
        }

        #endregion

        [Test]
        public void BuildReaderSource()
        {
            IMock engineMock = new DynamicMock(typeof (IDatabaseEngine));
            StubbedReaderSource stubbedReaderSource = new StubbedReaderSource(new TableDataReader(new DataTable()));

            ObjectFactory.InjectStub(typeof (IReaderSource), stubbedReaderSource);

            CommandFactory factory = new CommandFactory((IDatabaseEngine) engineMock.MockInstance);

            string theName = "Name Of The Command";
            IReaderSource readerSource = factory.BuildReaderSource(theName);

            Assert.IsTrue(stubbedReaderSource.WasInitialized);
            Assert.AreEqual(theName, readerSource.Name);
            Assert.AreSame(readerSource, stubbedReaderSource);
        }

        [Test]
        public void CreateCommand()
        {
            IMock engineMock = new DynamicMock(typeof (IDatabaseEngine));
            StubbedCommand stubbedCommand = new StubbedCommand();

            ObjectFactory.InjectStub(typeof (ICommand), stubbedCommand);

            CommandFactory factory = new CommandFactory((IDatabaseEngine) engineMock.MockInstance);

            string theCommandName = "Name Of The Command";
            ICommand command = factory.BuildCommand(theCommandName);

            Assert.IsTrue(stubbedCommand.WasInitialized);
            Assert.AreEqual(theCommandName, command.Name);
            Assert.AreSame(command, stubbedCommand);
        }
    }
}