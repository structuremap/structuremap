using StructureMap.Configuration;

namespace StructureMap.Client.Views
{
    [PluginFamily]
    public interface IViewPart
    {
        void WriteHTML(HTMLBuilder builder, GraphObject subject);
    }
}