namespace Lisp.Interface
{
    public interface ILambda : IAtom
    {
        ISExpression Evaluate(IEnvironment environment, IList list);
    }
}
