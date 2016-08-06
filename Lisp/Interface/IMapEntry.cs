namespace Lisp.Interface
{
    public interface IMapEntry : IValue
    {
        ISExpression Key { get; }
        ISExpression Value { get; }
    }
}
