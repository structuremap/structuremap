using System.Linq;
using NUnit.Framework;

namespace StructureMap.Testing
{
	[TestFixture]
	public class Resolving_mixed_open_and_closed_generic_types
	{
		[Test]
		public void when_only_open_instances_are_present_they_are_resolved()
		{
			var container = new Container(x =>
			{
				x.For(typeof(INeedAnEntity<>)).Add(typeof(OpenEntityType<>));
				x.For(typeof(INeedAnEntity<>)).Add(typeof(AnotherOpenEntityType<>));
			});

			var instances = container.GetAllInstances<INeedAnEntity<Entity>>().ToArray();

			instances.Length.ShouldEqual(2);
			instances[0].GetType().Name.ShouldEqual("OpenEntityType`1");
			instances[1].GetType().Name.ShouldEqual("AnotherOpenEntityType`1");
		}

		[Test]
		public void when_a_closed_instance_is_present_only_closed_types_are_resolved()
		{
			var container = new Container(x =>
			{
				x.For(typeof(INeedAnEntity<>)).Add(typeof(OpenEntityType<>));
				x.For(typeof(INeedAnEntity<Entity>)).Add(typeof(ClosedEntityType));
				x.For(typeof(INeedAnEntity<Entity>)).Add(typeof(AnotherClosedEntityType));
			});

			var instances = container.GetAllInstances<INeedAnEntity<Entity>>().ToArray();

			instances.Length.ShouldEqual(2);
			instances[0].GetType().Name.ShouldEqual("ClosedEntityType");
			instances[1].GetType().Name.ShouldEqual("AnotherClosedEntityType");
		}
	}

	public class Entity { }

	public interface INeedAnEntity<T>
	{
	}

	public class ClosedEntityType : INeedAnEntity<Entity>
	{
	}

	public class AnotherClosedEntityType : INeedAnEntity<Entity>
	{
	}

	public class OpenEntityType<T> : INeedAnEntity<T>
	{
	}

	public class AnotherOpenEntityType<T> : INeedAnEntity<T>
	{
	}

}