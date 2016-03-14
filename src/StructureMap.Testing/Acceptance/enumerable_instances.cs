using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class enumerable_instances
    {
        // SAMPLE: EnumerableFamilyPolicy_in_action
        [Fact]
        public void collection_types_are_all_possible_by_default()
        {
            // NOTE that we do NOT make any explicit registration of
            // IList<IWidget>, IEnumerable<IWidget>, ICollection<IWidget>, or IWidget[]
            var container = new Container(_ =>
            {
                _.For<IWidget>().Add<AWidget>();
                _.For<IWidget>().Add<BWidget>();
                _.For<IWidget>().Add<CWidget>();
            });

            // IList<T>
            container.GetInstance<IList<IWidget>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));

            // ICollection<T>
            container.GetInstance<ICollection<IWidget>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));

            // Array of T
            container.GetInstance<IWidget[]>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }

        // ENDSAMPLE

        // SAMPLE: explicit-enumeration-behavior
        [Fact]
        public void override_enumeration_behavior()
        {
            var container = new Container(_ =>
            {
                _.For<IWidget>().Add<AWidget>();
                _.For<IWidget>().Add<BWidget>();
                _.For<IWidget>().Add<CWidget>();

                // Explicit registration should have precedence over the default
                // behavior
                _.For<IWidget[]>().Use(new IWidget[] { new DefaultWidget() });
            });

            container.GetInstance<IWidget[]>()
                .Single().ShouldBeOfType<DefaultWidget>();
        }

        // ENDSAMPLE

        // SAMPLE: IWidgetValidator-enumerable
        public interface IWidgetValidator
        {
            IEnumerable<string> Validate(IWidget widget);
        }

        public class WidgetProcessor
        {
            private readonly IEnumerable<IWidgetValidator> _validators;

            public WidgetProcessor(IEnumerable<IWidgetValidator> validators)
            {
                _validators = validators;
            }

            public void Process(IWidget widget)
            {
                var validationMessages = _validators.SelectMany(x => x.Validate(widget))
                    .ToArray();

                if (validationMessages.Any())
                {
                    // don't process the widget
                }
            }
        }

        // ENDSAMPLE
    }
}