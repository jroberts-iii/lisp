namespace Lisp.Interface
{
    public interface IPair : IAtom
    {
        object Key { get; }
        object Value { get; }
    }
}
