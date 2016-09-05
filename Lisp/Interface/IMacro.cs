namespace Lisp.Interface
{
    public interface IMacro : IAtom
    {
        ISExpression Body { get; }
        string[] ParameterNames { get; }
        ISExpression Evaluate(IEnvironment environment, IList list);
    }
}
