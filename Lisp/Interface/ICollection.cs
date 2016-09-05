namespace Lisp.Interface
{
    public interface ICollection : ISExpression
    {
        bool IsEmpty { get; }
        IList ToList();
    }
}
