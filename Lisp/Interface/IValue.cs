namespace Lisp.Interface
{
    public interface IValue : IAtom
    {
    }

    public interface IValue<out T> : IValue
    {
        T Val { get; }
    }
}
