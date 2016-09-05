namespace Lisp.Interface
{
    public interface ILambda : IAtom
    {
        IEnvironment ClosureEnvironment { get; }
        string[] ParameterNames { get; }
        ISExpression Evaluate(IEnvironment environment, IList list);
    }
}
