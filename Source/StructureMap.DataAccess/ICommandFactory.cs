namespace StructureMap.DataAccess
{
    [PluginFamily("Default")]
    public interface ICommandFactory
    {
        ICommand BuildCommand(string commandName);
        IReaderSource BuildReaderSource(string name);
    }
}