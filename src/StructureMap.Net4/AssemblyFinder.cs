using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StructureMap.Graph
{
    public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindDependentAssemblies()
        {
            return FindAssemblies(file => { }).Where(x => x.GetReferencedAssemblies().Any(assem => assem.Name == "FubuMVC.Core"));
        }

        public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = FindBinPath();
            if (!string.IsNullOrEmpty(binPath))
            {
                assemblyPath = Path.Combine(assemblyPath, binPath);
            }


            return FindAssemblies(assemblyPath, logFailure);
        }

        public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, Action<string> logFailure)
        {
            var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
            var exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories);

            var files = dllFiles.Concat(exeFiles);
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                try
                {
                    assembly = AppDomain.CurrentDomain.Load(name);
                }
                catch (Exception)
                {
                    logFailure(file);
                }

                if (assembly != null) yield return assembly;
            }
        }

        public static string FindBinPath()
        {
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (!string.IsNullOrEmpty(binPath))
            {
                return Path.IsPathRooted(binPath)
                    ? binPath
                    : Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, binPath);
            }

            return null;
        }


        public static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter,
            Action<string> onDirectoryFound = null)
        {
            if (onDirectoryFound == null)
            {
                onDirectoryFound = dir => { };
            }

            return FindAssemblies(file => { }).Where(filter);
        }


    }
}