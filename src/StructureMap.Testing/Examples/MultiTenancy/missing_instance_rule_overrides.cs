using Shouldly;
using Xunit;

namespace StructureMap.Testing.Examples.MultiTenancy
{
    public class missing_instance_rule_overrides
    {
        // SAMPLE: missing-instance-mt-domain
        public interface Rule
        {
        }

        public class DefaultRule : Rule
        {
        }

        public class Client1Rule : Rule
        {
        }

        public class Client2Rule : Rule
        {
        }

        // ENDSAMPLE

        // SAMPLE: missing-instance-mt-fallthrough
        [Fact]
        public void use_customer_overrides()
        {
            var container = new Container(_ =>
            {
                _.For<Rule>().MissingNamedInstanceIs.Type<DefaultRule>();

                _.For<Rule>().Add<Client1Rule>().Named("client1");
                _.For<Rule>().Add<Client2Rule>().Named("client2");
            });

            // Client1 & Client2 have explicit registrations
            container.GetInstance<Rule>("client1").ShouldBeOfType<Client1Rule>();
            container.GetInstance<Rule>("client2").ShouldBeOfType<Client2Rule>();

            // Client3 has no explicit registration, so falls through to
            // DefaultRule
            container.GetInstance<Rule>("client3").ShouldBeOfType<DefaultRule>();
        }

        // ENDSAMPLE

        // SAMPLE: missing-instance-mt-lookup
        public interface IClientRulesRepsitory
        {
            ClientRulesData Find(string clientName);
        }

        public class ClientRulesData
        {
        }

        public class DataUsingRule : Rule
        {
            private readonly ClientRulesData _data;

            public DataUsingRule(ClientRulesData data)
            {
                _data = data;
            }
        }

        [Fact]
        public void register_by_looking_up_data()
        {
            var container = new Container(_ =>
            {
                _.For<Rule>().MissingNamedInstanceIs.ConstructedBy(
                    "Building client rules by looking up client data", c =>
                    {
                        var data = c.GetInstance<IClientRulesRepsitory>()
                            .Find(c.RequestedName);

                        return new DataUsingRule(data);
                    });
            });
        }

        // ENDSAMPLE
    }
}