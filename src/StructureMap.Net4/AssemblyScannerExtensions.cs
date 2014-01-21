using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap
{
    public static class AssemblyScannerExtensions
    {
        public static void TheCallingAssembly(this IAssemblyScanner scanner)
        {
            var callingAssembly = findTheCallingAssembly();

            if (callingAssembly != null)
            {
                scanner.Assembly(callingAssembly);
            }
        }

        public static void AssembliesFromApplicationBaseDirectory(this IAssemblyScanner scanner)
        {
            scanner.AssembliesFromApplicationBaseDirectory(a => true);
        }

        public static void AssembliesFromApplicationBaseDirectory(this IAssemblyScanner scanner, Predicate<Assembly> assemblyFilter)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            scanner.AssembliesFromPath(baseDirectory, assemblyFilter);
            var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
            if (Directory.Exists(binPath))
            {
                scanner.AssembliesFromPath(binPath, assemblyFilter);
            }
        }

        public static void AssembliesFromPath(this IAssemblyScanner scanner, string path)
        {
            scanner.AssembliesFromPath(path, a => true);
        }

        public static void AssembliesFromPath(this IAssemblyScanner scanner, string path,
            Predicate<Assembly> assemblyFilter)
        {
            var assemblyPaths = Directory.GetFiles(path)
                .Where(file =>
                    Path.GetExtension(file).Equals(
                        ".exe",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    Path.GetExtension(file).Equals(
                        ".dll",
                        StringComparison.OrdinalIgnoreCase));

            foreach (var assemblyPath in assemblyPaths)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.LoadFrom(assemblyPath);
                }
                catch
                {
                }
                if (assembly != null && assemblyFilter(assembly)) scanner.Assembly(assembly);
            }
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }
    }
}