using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_322_class_with_multiple_constructors
    {
        [Test]
        public void reproduce()
        {
            var anotherDependency = new AnotherDependency();

            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AssembliesFromApplicationBaseDirectory();
                    s.WithDefaultConventions();
                });
                x.For<IActivityService>().Singleton();

                
                x.ForConcreteType<ClassWithTwoConstructors>()
                    .Configure.SelectConstructor(() => new ClassWithTwoConstructors(new AnotherDependency()))
                    .Ctor<AnotherDependency>().Is(anotherDependency);

                    // You don't need to specify the argument name if there is only 1 for that type
                    //.Ctor<AnotherDependency>("anotherDependency").Is(anotherDependency);

            });

            container.GetInstance<MainService>()
                .AnotherDependency.AnotherDependency.ShouldBeTheSameAs(anotherDependency);
        }


        public class AnotherDependency
        {
            public AnotherDependency()
            {
            }
        }

        public class ClassWithTwoConstructors
        {
            private readonly AnotherDependency _anotherDependency;


            public ClassWithTwoConstructors(int age, string name)
            {

            }

            public ClassWithTwoConstructors(AnotherDependency anotherDependency)
            {
                _anotherDependency = anotherDependency;
            }

            public AnotherDependency AnotherDependency
            {
                get { return _anotherDependency; }
            }
        }

        public interface IStoryService { }

        public class StoryService : IStoryService
        {
        }

        public interface IActivityService { }
        public class ActivityService : IActivityService { }

        public class MainService
        {
            private readonly ClassWithTwoConstructors _anotherDependency;

            public MainService(IStoryService storyService,
                               ClassWithTwoConstructors anotherDependency)
            {
                _anotherDependency = anotherDependency;
            }

            public ClassWithTwoConstructors AnotherDependency
            {
                get { return _anotherDependency; }
            }
        }
    }





}