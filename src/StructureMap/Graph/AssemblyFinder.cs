using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StructureMap.Graph
{
    public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure, bool includeExeFiles)
        {
#if NET45
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            if (string.IsNullOrEmpty(binPath))
            {
                return FindAssemblies(assemblyPath, logFailure, includeExeFiles);
            }


            if (Path.IsPathRooted(binPath))
            {
                return FindAssemblies(binPath, logFailure, includeExeFiles);
            }


            var binPaths = binPath.Split(';');
            return binPaths.SelectMany(bin =>
            {
                var path = Path.Combine(assemblyPath, bin);
                return FindAssemblies(path, logFailure, includeExeFiles);
            });
#else
            var path = Path.GetFullPath("");
            return FindAssemblies(path, logFailure, includeExeFiles);
#endif


        }

        public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, Action<string> logFailure, bool includeExeFiles)
        {
            var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
            var files = dllFiles;

            if(includeExeFiles)
            {
                var exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories);
                files = dllFiles.Concat(exeFiles);
            }

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                try
                {
#if NET45
                    assembly = AppDomain.CurrentDomain.Load(name);
#else
                    assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(name));
#endif
                }
                catch (Exception)
                {
                    try
                    {
#if NET45
                        assembly = Assembly.LoadFrom(file);
#else
                        assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
#endif
                    }
                    catch (Exception)
                    {
                        logFailure(file);
                    }
                }

                if (assembly != null)
                {
                    yield return assembly;
                }
            }
        }




        public static IEnumerable<Assembly> FindAssemblies(Func<Assembly, bool> filter,
            Action<string> onDirectoryFound = null, bool includeExeFiles=false)
        {
            if (filter == null)
            {
                filter = a => true;
            }

            if (onDirectoryFound == null)
            {
                onDirectoryFound = dir => { };
            }

            return FindAssemblies(file => { }, includeExeFiles: includeExeFiles).Where(filter);
        }
    }
}
