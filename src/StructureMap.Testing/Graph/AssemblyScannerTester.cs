using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Testing.DocumentationExamples;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.Testing.Widget5;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Graph
{
    public class TestingRegistry : Registry
    {
        public static bool WasUsed;

        public TestingRegistry()
        {
            WasUsed = true;

            For<Rule>().Use(new ColorRule("Green"));
        }

        public static void Reset()
        {
            WasUsed = false;
        }
    }

    [TestFixture]
    public class AssemblyScannerTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            TestingRegistry.Reset();

            theGraph = null;
        }

        #endregion

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            string binFolder = Path.GetDirectoryName(GetType().Assembly.Location);
            assemblyScanningFolder = Path.Combine(binFolder, "DynamicallyLoaded");
            if (!Directory.Exists(assemblyScanningFolder)) Directory.CreateDirectory(assemblyScanningFolder);

            string assembly1 = typeof (RedGreenRegistry).Assembly.Location;
            string assembly2 = typeof (IWorker).Assembly.Location;

            File.Copy(assembly1, Path.Combine(assemblyScanningFolder, Path.GetFileName(assembly1)), true);
            File.Copy(assembly2, Path.Combine(assemblyScanningFolder, Path.GetFileName(assembly2)), true);
        }

        private PluginGraph theGraph;
        private string assemblyScanningFolder;

        private void Scan(Action<AssemblyScanner> action)
        {
            var scanner = new AssemblyScanner();
            action(scanner);
            theGraph = new PluginGraph();
            scanner.ExcludeNamespaceContainingType<ScanningRegistry>();
            scanner.ShouldBeOfType<IPluginGraphConfiguration>().Configure(theGraph);
            theGraph.Log.AssertFailures();
        }


        private void shouldHaveFamily<T>()
        {
            theGraph.PluginFamilies.Contains(typeof (T)).ShouldBeTrue();
        }

        private void shouldNotHaveFamily<T>()
        {
            theGraph.PluginFamilies.Contains(typeof (T)).ShouldBeFalse();
        }


        private void shouldHaveFamilyWithSameName<T>()
        {
            // The Types may not be "Equal" if their assemblies were loaded in different load contexts (.LoadFrom)
            // so we will consider them equal if their names match.
            theGraph.PluginFamilies.Any(family => family.PluginType.FullName == typeof (T).FullName).ShouldBeTrue();
        }

        private void shouldNotHaveFamilyWithSameName<T>()
        {
            theGraph.PluginFamilies.Any(family => family.PluginType.FullName == typeof (T).FullName).ShouldBeFalse();
        }

        [Test]
        public void AssemblyScanner_will_scan_for_attributes_by_default()
        {
            Scan(x => { x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>(); });

            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Test]
        public void is_in_namespace()
        {
            GetType().IsInNamespace("blah").ShouldBeFalse();
            GetType().IsInNamespace("StructureMap").ShouldBeTrue();
            GetType().IsInNamespace("StructureMap.Testing").ShouldBeTrue();
            GetType().IsInNamespace("StructureMap.Testing.Graph").ShouldBeTrue();
            GetType().IsInNamespace("StructureMap.Testing.Graph.Something").ShouldBeFalse();
        }

        [Test]
        public void class_outside_namespace_doesnt_match_any_namespace_check()
        {
            typeof(class_outside_namespace).IsInNamespace("blah").ShouldBeFalse();
            typeof(class_outside_namespace).IsInNamespace("StructureMap").ShouldBeFalse();
        }

        [Test]
        public void Only_scan_for_registries_ignores_attributes()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.IgnoreStructureMapAttributes();
            });

            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Test]
        public void scan_all_assemblies_in_a_folder()
        {
            Scan(x => x.AssembliesFromPath(assemblyScanningFolder));
            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldHaveFamilyWithSameName<IWorker>();
        }

        [Test, Explicit]
        public void scan_all_assemblies_in_application_base_directory()
        {
            Scan(x => x.AssembliesFromApplicationBaseDirectory());
            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldHaveFamilyWithSameName<IWorker>();
        }

        [Test]
        public void scan_but_ignore_registries_by_default()
        {
            Scan(x => { x.TheCallingAssembly(); });

            TestingRegistry.WasUsed.ShouldBeFalse();
        }


        [Test]
        public void scan_specific_assemblies_in_a_folder()
        {
            string assemblyToSpecificallyExclude = typeof (IWorker).Assembly.GetName().Name;
            Scan(
                x =>
                x.AssembliesFromPath(assemblyScanningFolder, asm => asm.GetName().Name != assemblyToSpecificallyExclude));

            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldNotHaveFamilyWithSameName<IWorker>();
        }

        [Test]
        public void scan_specific_assemblies_in_application_base_directory()
        {
            string assemblyToSpecificallyExclude = typeof (IWorker).Assembly.GetName().Name;
            Scan(
                x =>
                x.AssembliesFromPath(assemblyScanningFolder, asm => asm.GetName().Name != assemblyToSpecificallyExclude));

            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldNotHaveFamilyWithSameName<IWorker>();
        }

        [Test]
        public void Search_for_registries_when_explicitly_told()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.LookForRegistries();
            });

            TestingRegistry.WasUsed.ShouldBeTrue();
        }

        [Test]
        public void test_the_family_attribute_scanner()
        {
            var scanner = new FamilyAttributeScanner();
            var graph = new PluginGraph();

            var registry = new Registry();

            scanner.Process(typeof (ITypeThatHasAttributeButIsNotInRegistry), registry);
            registry.ShouldBeOfType<IPluginGraphConfiguration>().Configure(graph);

            graph.PluginFamilies.Contains(typeof (ITypeThatHasAttributeButIsNotInRegistry)).ShouldBeTrue();

            graph = new PluginGraph();
            registry = new Registry();

            scanner.Process(GetType(), registry);
            registry.ShouldBeOfType<IPluginGraphConfiguration>().Configure(graph);

            graph.PluginFamilies.Contains(GetType()).ShouldBeFalse();
        }

        [Test]
        public void use_a_dual_exclude()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Exclude(type => type == typeof (ITypeThatHasAttributeButIsNotInRegistry));
                x.Exclude(type => type == typeof (IInterfaceInWidget5));
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void use_a_dual_exclude2()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Exclude(type => type == typeof (ITypeThatHasAttributeButIsNotInRegistry));
                x.Exclude(type => type == GetType());
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Test]
        public void use_a_single_exclude()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Exclude(type => type == typeof (ITypeThatHasAttributeButIsNotInRegistry));
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Test]
        public void use_a_single_exclude_of_type()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.ExcludeType<ITypeThatHasAttributeButIsNotInRegistry>();
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void use_a_single_exclude2()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.ExcludeNamespace("StructureMap.Testing.Widget5");
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void use_a_single_exclude3()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.ExcludeNamespaceContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void Use_a_single_include_predicate()
        {
            Scan(x => { x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>(); });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();

            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Include(type => type == typeof (ITypeThatHasAttributeButIsNotInRegistry));
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void Use_a_single_include_predicate_2()
        {
            Scan(x => { x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>(); });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();

            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.IncludeNamespace(typeof (ITypeThatHasAttributeButIsNotInRegistry).Namespace);
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void Use_a_single_include_predicate_3()
        {
            Scan(x => { x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>(); });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();

            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.IncludeNamespaceContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void use_two_predicates_for_includes()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Include(type => type == typeof (ITypeThatHasAttributeButIsNotInRegistry));
                x.Include(type => type == typeof (IInterfaceInWidget5));
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }


        [Test]
        public void use_two_predicates_for_includes2()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Include(type => type == typeof (ITypeThatHasAttributeButIsNotInRegistry));
                x.Include(type => type == GetType());
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }
    }


    public interface IController
    {
    }

    public class AddressController : IController
    {
    }

    public class SiteController : IController
    {
    }

    [TestFixture]
    public class when_attaching_types_with_naming_pattern
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.Scan(o =>
                {
                    o.TheCallingAssembly();
                    o.AddAllTypesOf<IController>().NameBy(type => type.Name.Replace("Controller", ""));
                });
            });
        }

        #endregion

        private IContainer container;

        [Test]
        public void can_find_objects_later_by_name()
        {
            container.GetInstance<IController>("Address")
                .ShouldBeOfType<AddressController>();

            container.GetInstance<IController>("Site")
                .ShouldBeOfType<SiteController>();
        }
    }
}

public class class_outside_namespace
{
    
}