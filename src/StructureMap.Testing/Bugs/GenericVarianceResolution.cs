using System.Diagnostics;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Bugs
{
    using System.Linq;
    using NUnit.Framework;
    using StructureMap.Graph;



    [TestFixture]
    public class GenericVarianceResolution
    {
        public interface INotificationHandler<in TNotification>
        {
            void Handle(TNotification notification);
        }

        public class BaseNotificationHandler : INotificationHandler<object>
        {
            public void Handle(object notification)
            {

            }
        }

        public class OpenNotificationHandler<TNotification> : INotificationHandler<TNotification>
        {
            public void Handle(TNotification notification)
            {

            }
        }

        public class Notification { }

        public class ConcreteNotificationHandler : INotificationHandler<Notification>
        {
            public void Handle(Notification notification)
            {

            }
        }

        [Test]
        public void RegisterMultipleHandlersOfSameInterface()
        {
            typeof(OpenNotificationHandler<Notification>).CanBeCastTo<INotificationHandler<Notification>>()
                .ShouldBeTrue();

            typeof (OpenNotificationHandler<>).CanBeCastTo(typeof (INotificationHandler<>))
                .ShouldBeTrue();

            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.ConnectImplementationsToTypesClosing(typeof (INotificationHandler<>));
                });
            });

            var handlers = container.GetAllInstances<INotificationHandler<Notification>>();

            handlers.Select(x => x.GetType()).OrderBy(x => x.Name)
                .Each(x => Debug.WriteLine(x.Name))
                .ShouldHaveTheSameElementsAs(typeof(BaseNotificationHandler), typeof(ConcreteNotificationHandler), typeof(OpenNotificationHandler<Notification>));

        }
    }
}