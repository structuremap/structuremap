namespace StructureMap.DebuggerVisualizers.Testing
{
    public interface IDoThis
    {
        void Do();
    }

    public interface IDoThat
    {
        void DoIt();
    }

    public interface IDoMore<T>
    {
        void DoDo(T thing);
    }

    public interface IHasTwoGenerics<FIRST, SECOND>
    {
        SECOND DoIt(FIRST input);
    }

    public class DoThis : IDoThis
    {
        public void Do()
        {
            
        }
    }

    public class DoThat : IDoThat
    {
        public void DoIt()
        {
            
        }
    }

    public class DoMore<T> :IDoMore<T>
    {
        public void DoDo(T thing)
        {
            
        }
    }

    public class HasTwoGenerics<FIRST, SECOND> : IHasTwoGenerics<FIRST, SECOND>
    {
        public SECOND DoIt(FIRST input)
        {
            throw new System.NotImplementedException();
        }
    }

    public class DoForStrings :IDoMore<string>
    {
        public void DoDo(string thing)
        {
            

        }
    }
}