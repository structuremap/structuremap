using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Bugs
{
	public class OpenGenericDecoratorsWithConstructorPolicy_Error
	{
		[Fact]
		public void applies_the_constructor_policy_to_decorated_instances()
		{
			var container = new Container(_ =>
			{
				_.For(typeof(ICmdHandler<>)).DecorateAllWith(typeof(CmdDecorator<>));
				_.Policies.Add<TestLoggerConstructorPolicy>();
				_.Scan(a =>
				{
					a.TheCallingAssembly();
					a.ConnectImplementationsToTypesClosing(typeof(ICmdHandler<>));
				});
			});

			Console.WriteLine(container.WhatDoIHave());

			var handler = container.GetInstance<ICmdHandler<TestCmd>>();
		}
	}

	public interface ICmdHandler<T>
	{
		void Handle(T cmd);
	}

	public class CmdHandler : ICmdHandler<TestCmd>
	{
		public void Handle(TestCmd cmd)
		{
		}
	}

	public class TestCmd { }

	public class CmdDecorator<T> : ICmdHandler<T>
	{
		private readonly ICmdHandler<T> _decorated;
		private readonly ITestLogger _logger;

		public CmdDecorator(ICmdHandler<T> decorated, ITestLogger logger)
		{
			_decorated = decorated;
			_logger = logger;
		}

		public void Handle(T cmd)
		{
			_decorated.Handle(cmd);
		}
	}

	public interface ITestLogger
	{
	}

	public class TestLogger
	{
	}

	public class TestLoggerConstructorPolicy : ConfiguredInstancePolicy
	{
		protected override void apply(Type pluginType, IConfiguredInstance instance)
		{
			var param = instance.Constructor.GetParameters().FirstOrDefault(x => x.ParameterType == typeof(ITestLogger));
			if (param != null)
			{
				var logger = new TestLogger();
				instance.Dependencies.AddForConstructorParameter(param, logger);
			}
			else
			{
				// Try to inject an ILogger via public-settable property.
				var prop = instance.SettableProperties().FirstOrDefault(x => x.PropertyType == typeof(ITestLogger));
				if (prop != null)
				{
					var logger = new TestLogger();
					instance.Dependencies.AddForProperty(prop, logger);
				}
			}
		}
	}
}
