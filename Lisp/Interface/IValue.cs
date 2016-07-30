namespace Lisp.Interface
{
    public interface IValue : IAtom
    {
        object Val { get; }
    }
}
