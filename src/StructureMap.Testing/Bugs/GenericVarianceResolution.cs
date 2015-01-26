namespace StructureMap.Testing.Bugs
{
    using System.Linq;
    using NUnit.Framework;
    using StructureMap.Graph;

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

    [TestFixture]
    public class GenericVarianceResolution
    {
        [Test]
        public void RegisterMultipleHandlersOfSameInterface()
        {
            var container = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AddAllTypesOf(typeof (INotificationHandler<>));
                });
            });

            var handlers = container.GetAllInstances<INotificationHandler<Notification>>();

            handlers.Single(h => h.GetType() == typeof (ConcreteNotificationHandler));
            //handlers.Single(h => h.GetType() == typeof(OpenNotificationHandler<Notification>));
            handlers.Single(h => h.GetType() == typeof(BaseNotificationHandler));

            handlers.Count().ShouldEqual(3);
        }
    }
}