using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;
using StructureMap.Testing.DocumentationExamples;
using StructureMap.Testing.ExeWidget;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.Testing.Widget5;
using StructureMap.TypeRules;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

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

    public class AssemblyScannerTesterFixture
    {
        public AssemblyScannerTesterFixture()
        {
            var binFolder = Path.GetDirectoryName(GetType().Assembly.Location);
            AssemblyScanningFolder = Path.Combine(binFolder, "DynamicallyLoaded");
            if (!Directory.Exists(AssemblyScanningFolder)) Directory.CreateDirectory(AssemblyScanningFolder);

            var assembly1 = typeof(RedGreenRegistry).Assembly.Location;
            var assembly2 = typeof(IWorker).Assembly.Location;
            var assembly3 = typeof(IDefinedInExe).Assembly.Location;

            File.Copy(assembly1, Path.Combine(AssemblyScanningFolder, Path.GetFileName(assembly1)), true);
            File.Copy(assembly2, Path.Combine(AssemblyScanningFolder, Path.GetFileName(assembly2)), true);
            File.Copy(assembly3, Path.Combine(AssemblyScanningFolder, Path.GetFileName(assembly3)), true);
        }

        public string AssemblyScanningFolder { get; private set; }
    }

    public class AssemblyScannerTester : IClassFixture<AssemblyScannerTesterFixture>
    {
        public AssemblyScannerTester(AssemblyScannerTesterFixture assemblyScannerTesterFixture)
        {
            TestingRegistry.Reset();

            theGraph = null;
            assemblyScanningFolder = assemblyScannerTesterFixture.AssemblyScanningFolder;
        }

        private PluginGraph theGraph;
        private string assemblyScanningFolder;

        private void Scan(Action<IAssemblyScanner> action)
        {
            var registry = new Registry();
            registry.Scan(scan =>
            {
                action(scan);

                scan.ExcludeNamespaceContainingType<ScanningRegistry>();
                scan.Convention<FakeConvention>();
            });

            var builder = new PluginGraphBuilder();
            builder.Add(registry);
            theGraph = builder.Build();
        }

        public class FakeConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, Registry registry)
            {
                types.FindTypes(TypeClassification.Interfaces).Each(type => registry.For(type));
            }
        }

        private void shouldHaveFamily<T>()
        {
            theGraph.Families.Has(typeof(T)).ShouldBeTrue();
        }

        private void shouldNotHaveFamily<T>()
        {
            theGraph.Families.Has(typeof(T)).ShouldBeFalse();
        }

        private void shouldHaveFamilyWithSameName<T>()
        {
            // The Types may not be "Equal" if their assemblies were loaded in different load contexts (.LoadFrom)
            // so we will consider them equal if their names match.
            theGraph.Families.Any(family => family.PluginType.FullName == typeof(T).FullName).ShouldBeTrue();
        }

        private void shouldNotHaveFamilyWithSameName<T>()
        {
            theGraph.Families.Any(family => family.PluginType.FullName == typeof(T).FullName).ShouldBeFalse();
        }

        [Fact]
        public void is_in_namespace()
        {
            GetType().IsInNamespace("blah").ShouldBeFalse();
            GetType().IsInNamespace("Struct").ShouldBeFalse();
            GetType().IsInNamespace("StructureMap").ShouldBeTrue();
            GetType().IsInNamespace("StructureMap.Test").ShouldBeFalse();
            GetType().IsInNamespace("StructureMap.Testing").ShouldBeTrue();
            GetType().IsInNamespace("StructureMap.Testing.Graph").ShouldBeTrue();
            GetType().IsInNamespace("StructureMap.Testing.Graphics").ShouldBeFalse();
            GetType().IsInNamespace("StructureMap.Testing.Graph.Something").ShouldBeFalse();

            var _person = new
            {
                ID = 1,
                FirstName = "Michael",
                LastName = "Sync"
            };

            _person.GetType().IsInNamespace("foo").ShouldBeFalse();
        }

        [Fact]
        public void class_outside_namespace_doesnt_match_any_namespace_check()
        {
            typeof(class_outside_namespace).IsInNamespace("blah").ShouldBeFalse();
            typeof(class_outside_namespace).IsInNamespace("StructureMap").ShouldBeFalse();
        }

        // SAMPLE: scan-filesystem
        [Fact]
        public void scan_all_assemblies_in_a_folder()
        {
            Scan(x => x.AssembliesFromPath(assemblyScanningFolder));
            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldHaveFamilyWithSameName<IWorker>();
            shouldNotHaveFamilyWithSameName<IDefinedInExe>();
        }

        [Fact]
        public void scan_all_assemblies_in_application_base_directory()
        {
            Scan(x => x.AssembliesFromApplicationBaseDirectory());
            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldHaveFamilyWithSameName<IWorker>();
            shouldNotHaveFamilyWithSameName<IDefinedInExe>();
        }

        // ENDSAMPLE

        // SAMPLE: scan-filesystem-for-exe
        [Fact]
        public void scan_all_assemblies_in_a_folder_including_exe()
        {
            Scan(x => x.AssembliesAndExecutablesFromPath(assemblyScanningFolder));

            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldHaveFamilyWithSameName<IWorker>();
            shouldHaveFamilyWithSameName<IDefinedInExe>();
        }

        [Fact]
        public void scan_all_assemblies_in_application_base_directory_including_exe()
        {
            Scan(x => x.AssembliesAndExecutablesFromApplicationBaseDirectory());

            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldHaveFamilyWithSameName<IWorker>();
            shouldHaveFamilyWithSameName<IDefinedInExe>();
        }

        // ENDSAMPLE

        // SAMPLE: scan-calling-assembly
        [Fact]
        public void scan_but_ignore_registries_by_default()
        {
            Scan(x => { x.TheCallingAssembly(); });

            TestingRegistry.WasUsed.ShouldBeFalse();
        }

        // ENDSAMPLE

        [Fact]
        public void scan_specific_assemblies_in_a_folder()
        {
            var assemblyToSpecificallyExclude = typeof(IWorker).Assembly.GetName().Name;
            Scan(
                x =>
                    x.AssembliesFromPath(assemblyScanningFolder,
                        asm => asm.GetName().Name != assemblyToSpecificallyExclude));

            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldNotHaveFamilyWithSameName<IWorker>();
        }

        [Fact]
        public void scan_specific_assemblies_in_application_base_directory()
        {
            var assemblyToSpecificallyExclude = typeof(IWorker).Assembly.GetName().Name;
            Scan(
                x =>
                    x.AssembliesFromPath(assemblyScanningFolder,
                        asm => asm.GetName().Name != assemblyToSpecificallyExclude));

            shouldHaveFamilyWithSameName<IInterfaceInWidget5>();
            shouldNotHaveFamilyWithSameName<IWorker>();
        }

        // SAMPLE: scan-for-registries
        [Fact]
        public void Search_for_registries_when_explicitly_told()
        {
            Scan(x =>
            {
                x.TheCallingAssembly();
                x.LookForRegistries();
            });

            TestingRegistry.WasUsed.ShouldBeTrue();
        }

        // ENDSAMPLE

        [Fact]
        public void use_a_dual_exclude()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Exclude(type => type == typeof(ITypeThatHasAttributeButIsNotInRegistry));
                x.Exclude(type => type == typeof(IInterfaceInWidget5));
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Fact]
        public void use_a_dual_exclude2()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Exclude(type => type == typeof(ITypeThatHasAttributeButIsNotInRegistry));
                x.Exclude(type => type == GetType());
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Fact]
        public void use_a_single_exclude()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Exclude(type => type == typeof(ITypeThatHasAttributeButIsNotInRegistry));
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldNotHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        // SAMPLE: scan-exclusions
        [Fact]
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

        [Fact]
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

        [Fact]
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

        // ENDSAMPLE

        [Fact]
        public void Use_a_single_include_predicate()
        {
            Scan(x => { x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>(); });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();

            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Include(type => type == typeof(ITypeThatHasAttributeButIsNotInRegistry));
            });

            shouldNotHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Fact]
        public void Use_a_single_include_predicate_2()
        {
            Scan(x => { x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>(); });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();

            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.IncludeNamespace(typeof(ITypeThatHasAttributeButIsNotInRegistry).Namespace);
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Fact]
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

        [Fact]
        public void use_two_predicates_for_includes()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Include(type => type == typeof(ITypeThatHasAttributeButIsNotInRegistry));
                x.Include(type => type == typeof(IInterfaceInWidget5));
            });

            shouldHaveFamily<IInterfaceInWidget5>();
            shouldHaveFamily<ITypeThatHasAttributeButIsNotInRegistry>();
        }

        [Fact]
        public void use_two_predicates_for_includes2()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<ITypeThatHasAttributeButIsNotInRegistry>();
                x.Include(type => type == typeof(ITypeThatHasAttributeButIsNotInRegistry));
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

    public class when_attaching_types_with_naming_pattern
    {
        public when_attaching_types_with_naming_pattern()
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

        private IContainer container;

        [Fact]
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