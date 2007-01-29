namespace StructureMap.Client.Controllers
{
    public interface IHTMLSourceFactory
    {
        IHTMLSource GetSource(string viewName);
    }
}