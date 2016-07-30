namespace Lisp.Interface
{
    public interface IMapEntry : IAtom
    {
        ISExpression Key { get; }
        ISExpression Value { get; }
    }
}
