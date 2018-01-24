using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StructureMap.Graph
{
    public static class AssemblyFinder
    {
        public static IEnumerable<Assembly> FindAssemblies(Action<string> logFailure, bool includeExeFiles, Func<string, bool> pathFilter = null)
        {
#if NET45
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;

            if (string.IsNullOrEmpty(binPath))
            {
                return FindAssemblies(assemblyPath, logFailure, includeExeFiles, pathFilter);
            }


            if (Path.IsPathRooted(binPath))
            {
                return FindAssemblies(binPath, logFailure, includeExeFiles, pathFilter);
            }


            var binPaths = binPath.Split(';');
            return binPaths.SelectMany(bin =>
            {
                var path = Path.Combine(assemblyPath, bin);
                return FindAssemblies(path, logFailure, includeExeFiles, pathFilter);
            });
#else
            string path;
            try {
                path = AppContext.BaseDirectory;
            }
            catch (Exception) {
                path = System.IO.Directory.GetCurrentDirectory();
            }
            
            return FindAssemblies(path, logFailure, includeExeFiles, pathFilter);
#endif


        }

        public static IEnumerable<Assembly> FindAssemblies(string assemblyPath, Action<string> logFailure, bool includeExeFiles, Func<string, bool> pathFilter = null)
        {
            var dllFiles = Directory.EnumerateFiles(assemblyPath, "*.dll", SearchOption.AllDirectories);
            var files = dllFiles;

            if (includeExeFiles)
            {
                var exeFiles = Directory.EnumerateFiles(assemblyPath, "*.exe", SearchOption.AllDirectories);
                files = dllFiles.Concat(exeFiles);
            }

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Assembly assembly = null;

                if (pathFilter != null)
                {
                    if (!pathFilter(file))
                        continue;
                }

                try
                {
#if NET45 || NETSTANDARD2_0
                    assembly = AppDomain.CurrentDomain.Load(name);
#endif

#if NETSTANDARD1_3
                    assembly = Assembly.Load(new AssemblyName(name));
#endif

#if NETSTANDARD1_5
                    assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(name));
#endif
                }
                catch (Exception)
                {
                    try
                    {
#if NET45 || NETSTANDARD2_0
                        assembly = Assembly.LoadFrom(file);
#endif




#if NETSTANDARD1_5
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
            Func<string, bool> pathFilter = null, bool includeExeFiles = false)
        {
            if (filter == null)
            {
                filter = a => true;
            }

            return FindAssemblies(file => { }, includeExeFiles: includeExeFiles, pathFilter: pathFilter).Where(filter);
        }
    }
}
