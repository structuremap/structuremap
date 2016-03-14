using Shouldly;
using StructureMap.Graph.Scanning;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Graph.Scanning
{
    public static class TypeRepositoryTestExtensions
    {
        public static void ShouldHaveTypes(this IEnumerable<Type> expected, IEnumerable<Type> actual)
        {
            expected.OrderBy(x => x.FullName).ShouldHaveTheSameElementsAs(actual.OrderBy(x => x.FullName));
        }

        public static void ShouldHaveShelves(this IEnumerable<Type> expected, params IList<Type>[] shelves)
        {
            expected.ShouldHaveTypes(shelves.SelectMany(x => x).ToArray());
        }
    }

    public class TypeRepositoryTester
    {
        private AssemblyTypes theTypes;
        private Type[] theInners;

        public TypeRepositoryTester()
        {
            theInners = GetType().GetNestedTypes();

            theTypes = new AssemblyTypes("some name", () => theInners);
        }

        [Fact]
        public void assert_no_type_scanning_failures_happy_path()
        {
            TypeRepository.ClearAll();
            TypeRepository.FindTypes(GetType().Assembly, TypeClassification.All).Wait();

            // SAMPLE: assert-no-type-scanning-failures
            TypeRepository.AssertNoTypeScanningFailures();
            // ENDSAMPLE
        }

        [Fact]
        public void successful_assembly_types()
        {
            var types = new AssemblyTypes(typeof(IContainer).Assembly);
            types.Record.Name.ShouldBe(typeof(IContainer).Assembly.FullName);
            types.Record.LoadException.ShouldBeNull();
        }

        [Fact]
        public void failed_assembly_types()
        {
            var types = new AssemblyTypes("FakeOne", () => { throw new DivideByZeroException(); });

            types.Record.Name.ShouldBe("FakeOne");
            types.Record.LoadException.ShouldBeOfType<DivideByZeroException>();
        }

        [Fact]
        public void create_assembly_types_and_it_categorizes_stuff_for_you()
        {
            theTypes.ClosedTypes.Concretes.ShouldHaveTheSameElementsAs(typeof(Concrete1), typeof(Concrete2));
            theTypes.ClosedTypes.Interfaces.ShouldHaveTheSameElementsAs(typeof(Interface1), typeof(Interface2));
            theTypes.ClosedTypes.Abstracts.ShouldHaveTheSameElementsAs(typeof(Abstract1), typeof(Abstract2));

            theTypes.OpenTypes.Concretes.ShouldHaveTheSameElementsAs(typeof(OpenConcrete1<>), typeof(OpenConcrete2<>));
            theTypes.OpenTypes.Abstracts.ShouldHaveTheSameElementsAs(typeof(OpenAbstract1<>), typeof(OpenAbstract2<>));
            theTypes.OpenTypes.Interfaces.ShouldHaveTheSameElementsAs(typeof(OpenInterface1<>), typeof(OpenInterface2<>));
        }

        [Fact]
        public void assembly_shelf_select_lists()
        {
            var theShelf = theTypes.ClosedTypes;
            theShelf.SelectLists(TypeClassification.All).ShouldHaveTheSameElementsAs(theShelf.Interfaces, theShelf.Concretes, theShelf.Abstracts);
            theShelf.SelectLists(TypeClassification.Open).ShouldHaveTheSameElementsAs(theShelf.Interfaces, theShelf.Concretes, theShelf.Abstracts);
            theShelf.SelectLists(TypeClassification.Closed).ShouldHaveTheSameElementsAs(theShelf.Interfaces, theShelf.Concretes, theShelf.Abstracts);

            theShelf.SelectLists(TypeClassification.Interfaces).ShouldHaveTheSameElementsAs(theShelf.Interfaces);
            theShelf.SelectLists(TypeClassification.Concretes).ShouldHaveTheSameElementsAs(theShelf.Concretes);
            theShelf.SelectLists(TypeClassification.Abstracts).ShouldHaveTheSameElementsAs(theShelf.Abstracts);
            theShelf.SelectLists(TypeClassification.Abstracts | TypeClassification.Interfaces).ShouldHaveTheSameElementsAs(theShelf.Interfaces, theShelf.Abstracts);
            theShelf.SelectLists(TypeClassification.Abstracts | TypeClassification.Interfaces | TypeClassification.Concretes).ShouldHaveTheSameElementsAs(theShelf.Interfaces, theShelf.Concretes, theShelf.Abstracts);
        }

        [Fact]
        public void TypeRepository_mechanics()
        {
            var task = TypeRepository.FindTypes(GetType().Assembly, TypeClassification.Interfaces);
            task.Wait();

            task.Result.ShouldContain(typeof(Interface1));
            task.Result.ShouldContain(typeof(Interface2));
            task.Result.ShouldNotContain(typeof(Concrete1));
        }

        [Fact]
        public void find_type_set()
        {
            var widget1 = typeof(IWidget).Assembly;
            var widget2 = typeof(StructureMap.Testing.Widget2.Rule1).Assembly;
            var widget3 = typeof(StructureMap.Testing.Widget3.ColorService).Assembly;

            var task = TypeRepository.FindTypes(new[] { widget1, widget2, widget3 }, type => type.Name.Contains("Color"));
            task.Wait();

            var types = task.Result;

            /*
      ColorRule
ColorService
ColorWidget
ColorWidgetMaker
IColor
Widget1Color
Widget2Color
             */

            types.AllTypes().OrderBy(x => x.Name).Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("ColorRule", "ColorService", "ColorWidget", "ColorWidgetMaker", "IColor", "Widget1Color", "Widget2Color");

            types.FindTypes(TypeClassification.Interfaces).OrderBy(x => x.Name)
                .ShouldHaveTheSameElementsAs(typeof(IColor));
        }

        [Fact]
        public void find_all_types()
        {
            theTypes.FindTypes(TypeClassification.All).ShouldHaveTypes(theInners);
        }

        [Fact]
        public void find_all_interfaces()
        {
            theTypes.FindTypes(TypeClassification.Interfaces)
                .ShouldHaveTypes(theInners.Where(x => x.IsInterface));
        }

        [Fact]
        public void find_all_concretes()
        {
            theTypes.FindTypes(TypeClassification.Concretes)
                .ShouldHaveTypes(theInners.Where(x => x.IsClass && !x.IsAbstract));
        }

        [Fact]
        public void find_all_abstracts()
        {
            theTypes.FindTypes(TypeClassification.Abstracts)
                .ShouldHaveShelves(theTypes.OpenTypes.Abstracts, theTypes.ClosedTypes.Abstracts);
        }

        [Fact]
        public void find_all_open_types()
        {
            theTypes.FindTypes(TypeClassification.Open)
                .ShouldHaveTypes(theInners.Where(x => x.IsOpenGeneric()));
        }

        [Fact]
        public void find_all_closed_types()
        {
            theTypes.FindTypes(TypeClassification.Closed)
                .ShouldHaveTypes(theInners.Where(x => !x.IsOpenGeneric()));
        }

        [Fact]
        public void find_closed_interfaces()
        {
            theTypes.FindTypes(TypeClassification.Closed | TypeClassification.Interfaces)
                .ShouldHaveTypes(theTypes.ClosedTypes.Interfaces);
        }

        [Fact]
        public void find_open_interfaces()
        {
            theTypes.FindTypes(TypeClassification.Open | TypeClassification.Interfaces)
                .ShouldHaveTypes(theTypes.OpenTypes.Interfaces);
        }

        [Fact]
        public void find_open_and_closed_concretes()
        {
            theTypes.FindTypes(TypeClassification.Open | TypeClassification.Concretes)
                .ShouldHaveTypes(theTypes.OpenTypes.Concretes);

            theTypes.FindTypes(TypeClassification.Closed | TypeClassification.Concretes)
                .ShouldHaveTypes(theTypes.ClosedTypes.Concretes);
        }

        [Fact]
        public void find_open_and_closed_abstracts()
        {
            theTypes.FindTypes(TypeClassification.Open | TypeClassification.Abstracts)
                .ShouldHaveTypes(theTypes.OpenTypes.Abstracts);

            theTypes.FindTypes(TypeClassification.Closed | TypeClassification.Abstracts)
                .ShouldHaveTypes(theTypes.ClosedTypes.Abstracts);
        }

        [Fact]
        public void mixed_classification()
        {
            theTypes.FindTypes(TypeClassification.Closed | TypeClassification.Abstracts | TypeClassification.Interfaces)
                .ShouldHaveShelves(theTypes.ClosedTypes.Abstracts, theTypes.ClosedTypes.Interfaces);

            theTypes.FindTypes(TypeClassification.Abstracts | TypeClassification.Interfaces)
                .ShouldHaveShelves(theTypes.ClosedTypes.Abstracts, theTypes.ClosedTypes.Interfaces, theTypes.OpenTypes.Abstracts, theTypes.OpenTypes.Interfaces);
        }

        public class Concrete1 { }

        public class Concrete2 { }

        public abstract class Abstract1 { }

        public abstract class Abstract2 { }

        public interface Interface1 { }

        public interface Interface2 { }

        public class OpenConcrete1<T> { }

        public class OpenConcrete2<T> { }

        public abstract class OpenAbstract1<T> { }

        public abstract class OpenAbstract2<T> { }

        public interface OpenInterface1<T> { }

        public interface OpenInterface2<T> { }
    }
}