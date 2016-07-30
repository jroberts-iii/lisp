namespace Lisp.Interface
{
    public interface IMap : ICollection
    {
        bool Exists(ISExpression key);
        ISExpression Get(ISExpression key);
    }
}
