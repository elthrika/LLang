namespace Compiler1
{
    class Either<T1, T2>
    {
        internal readonly T1 First;
        internal readonly T2 Second;

        internal readonly bool FirstSet;
        internal readonly bool SecondSet;

        internal Either(T1 first)
        {
            First = first;
            FirstSet = true;
        }

        internal Either(T2 second)
        {
            Second = second;
            SecondSet = true;
        }
    }
}
