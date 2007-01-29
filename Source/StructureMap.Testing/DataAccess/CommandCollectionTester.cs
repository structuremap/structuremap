using NMock;
using NUnit.Framework;
using StructureMap.DataAccess;

namespace StructureMap.Testing.DataAccess
{
    [TestFixture]
    public class CommandCollectionTester
    {
        [Test]
        public void GetACommandThreeTimesGetTheSameCommand()
        {
            IMock factoryMock = new DynamicMock(typeof (ICommandFactory));
            string theCommandName = "name of command";

            StubbedCommand stubbedCommand = new StubbedCommand();

            factoryMock.ExpectAndReturn("BuildCommand", stubbedCommand, theCommandName);

            DataSession session = ObjectMother.MSSQLDataSession();
            CommandCollection commands = new CommandCollection(session, (ICommandFactory) factoryMock.MockInstance);

            Assert.AreSame(stubbedCommand, commands[theCommandName]);
            Assert.AreSame(stubbedCommand, commands[theCommandName]);
            Assert.AreSame(stubbedCommand, commands[theCommandName]);

            Assert.AreSame(session, stubbedCommand.Session);
        }
    }
}