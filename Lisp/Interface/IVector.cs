namespace Lisp.Interface
{
    public interface IVector : ICollection
    {
        ISExpression Get(int index);
        int Length();
    }
}
