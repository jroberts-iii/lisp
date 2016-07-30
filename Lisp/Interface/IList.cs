namespace Lisp.Interface
{
    public interface IList : ICollection
    {
        ISExpression First { get; }
        bool IsEmpty { get; }
        IList Rest { get; }

        IList Cons(ISExpression sExpression);
    }
}
