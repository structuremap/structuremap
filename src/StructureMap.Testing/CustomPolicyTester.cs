using Shouldly;
using StructureMap.Pipeline;
using System;
using System.Diagnostics;
using Xunit;

namespace StructureMap.Testing
{
    public class CustomPolicyTester
    {
        [Fact]
        public void use_a_custom_policy()
        {
            var container = new Container(_ =>
            {
                _.For<IRepository>().Use<NormalRepository>();

                _.Policies.Add<SpecialGuyGetsSpecialRepository>();
            });

            Debug.WriteLine(container.WhatDoIHave());

            // Policy should not apply to NormalGuy
            container.GetInstance<NormalGuy>().Repository.ShouldBeOfType<NormalRepository>();

            // Policy should override the repository dependency of SpecialGuy
            container.GetInstance<SpecialGuy>().Repository.ShouldBeOfType<SpecialRepository>();
        }

        public class SpecialGuyGetsSpecialRepository : ConfiguredInstancePolicy
        {
            protected override void apply(Type pluginType, IConfiguredInstance instance)
            {
                if (instance.PluggedType == typeof(SpecialGuy))
                {
                    instance.Dependencies.Add(typeof(IRepository), new SmartInstance<SpecialRepository>());
                }
            }
        }

        public interface IRepository { }

        public class NormalRepository : IRepository { }

        public class SpecialRepository : IRepository { }

        public class NormalGuy
        {
            public IRepository Repository { get; set; }

            public NormalGuy(IRepository repository)
            {
                Repository = repository;
            }
        }

        public class SpecialGuy
        {
            public IRepository Repository { get; set; }

            public SpecialGuy(IRepository repository)
            {
                Repository = repository;
            }
        }
    }
}