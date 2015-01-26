namespace StructureMap.Graph
{
    public class CompositeFilter<T>
    {
        private readonly CompositePredicate<T> _excludes = new CompositePredicate<T>();
        private readonly CompositePredicate<T> _includes = new CompositePredicate<T>();

        public CompositePredicate<T> Includes
        {
            get { return _includes; }
            set { }
        }

        public CompositePredicate<T> Excludes
        {
            get { return _excludes; }
            set { }
        }

        public bool Matches(T target)
        {
            return Includes.MatchesAny(target) && Excludes.DoesNotMatcheAny(target);
        }
    }
}