namespace Lisp.Interface
{
    public interface IValue : IAtom
    {
        IValue Addition(IValue value);
        IValue Division(IValue value);
        IValue Multiplication(IValue value);
        IValue Subtraction(IValue value);
        object ToObject();
    }
}
