using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
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

        public static void AssembliesFromApplicationBaseDirectory(this IAssemblyScanner scanner, Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(assemblyFilter, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: false);

            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }
        }

        /// <summary>
        /// Choosing this option will direct StructureMap to *also* scan files ending in '*.exe'
        /// </summary>
        /// <param name="scanner"></param>
        /// <param name="assemblyFilter"></param>
        /// <param name="includeExeFiles"></param>
        public static void AssembliesAndExecutablesFromApplicationBaseDirectory(this IAssemblyScanner scanner, Func<Assembly, bool> assemblyFilter = null)
        {
            var assemblies = AssemblyFinder.FindAssemblies(assemblyFilter, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: true);

            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }
        }

        public static void AssembliesAndExecutablesFromPath(this IAssemblyScanner scanner, string path)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: true);

            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }
        }

        public static void AssembliesFromPath(this IAssemblyScanner scanner, string path)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: false);

            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }
        }

        public static void AssembliesAndExecutablesFromPath(this IAssemblyScanner scanner, string path,
            Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: true).Where(assemblyFilter);


            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }
        }

        public static void AssembliesFromPath(this IAssemblyScanner scanner, string path,
            Func<Assembly, bool> assemblyFilter)
        {
            var assemblies = AssemblyFinder.FindAssemblies(path, txt =>
            {
                Console.WriteLine("StructureMap could not load assembly from file " + txt);
            }, includeExeFiles: false).Where(assemblyFilter);


            foreach (var assembly in assemblies)
            {
                scanner.Assembly(assembly);
            }
        }

        private static Assembly findTheCallingAssembly()
        {
            var trace = new StackTrace(false);

            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var smAssembly = typeof (Registry).Assembly;

            Assembly callingAssembly = null;
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != thisAssembly && assembly != smAssembly)
                {
                    callingAssembly = assembly;
                    break;
                }
            }
            return callingAssembly;
        }
    }
}