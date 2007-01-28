using System.Collections;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;

namespace StructureMap.DeploymentTasks
{
    public class PluginGraphFilter
    {
        private readonly PluginGraphReport _report;
        private ArrayList _assemblies = new ArrayList();
        private ArrayList _families = new ArrayList();
        private ArrayList _plugins = new ArrayList();

        public PluginGraphFilter(PluginGraphReport report)
        {
            _report = report;
        }

        public void FilterDeployment(string deploymentTarget)
        {
            filterAssemblies(deploymentTarget);
            filterFamilies(deploymentTarget);
        }

        private void filterFamilies(string deploymentTarget)
        {
            foreach (FamilyToken family in _report.Families)
            {
                if (!family.IsDeployed(deploymentTarget) || _assemblies.Contains(family.AssemblyName))
                {
                    _families.Add(family.FullPluginTypeName);
                }
                else
                {
                    filterPlugins(family);
                }
            }
        }

        private void filterPlugins(FamilyToken family)
        {
            foreach (PluginToken plugin in family.Plugins)
            {
                if (_assemblies.Contains(plugin.AssemblyName))
                {
                    _plugins.Add(plugin);
                }
            }
        }

        private void filterAssemblies(string deploymentTarget)
        {
            foreach (AssemblyToken assembly in _report.Assemblies)
            {
                if (!assembly.IsDeployed(deploymentTarget))
                {
                    _assemblies.Add(assembly.AssemblyName);
                }
            }
        }

        public string[] AssembliesToRemove
        {
            get { return (string[]) _assemblies.ToArray(typeof (string)); }
        }

        public string[] FamiliesToRemove
        {
            get { return (string[]) _families.ToArray(typeof (string)); }
        }

        public PluginToken[] PluginsToRemove
        {
            get { return (PluginToken[]) _plugins.ToArray(typeof (PluginToken)); }
        }
    }
}