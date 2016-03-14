using StructureMap.Graph;
using StructureMap.TypeRules;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Bugs
{
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

        public class Notification
        {
        }

        public class ConcreteNotificationHandler : INotificationHandler<Notification>
        {
            public void Handle(Notification notification)
            {
            }
        }

        [Fact]
        public void RegisterMultipleHandlersOfSameInterface()
        {
            typeof(OpenNotificationHandler<Notification>).CanBeCastTo<INotificationHandler<Notification>>()
                .ShouldBeTrue();

            typeof(OpenNotificationHandler<>).CanBeCastTo(typeof(INotificationHandler<>))
                .ShouldBeTrue();

            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });
            });

            var handlers = container.GetAllInstances<INotificationHandler<Notification>>();

            handlers.Select(x => x.GetType()).OrderBy(x => x.Name)
                .Each(x => Debug.WriteLine(x.Name))
                .ShouldHaveTheSameElementsAs(typeof(BaseNotificationHandler), typeof(ConcreteNotificationHandler),
                    typeof(OpenNotificationHandler<Notification>));
        }
    }
}