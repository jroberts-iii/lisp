namespace Lisp.Interface
{
    public interface ICollection : ISExpression
    {
        IList ToList();
    }
}
