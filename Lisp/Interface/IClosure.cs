namespace Lisp.Interface
{
    public interface IClosure : ILambda
    {
        IEnvironment ClosureEnvironment { get; }
    }
}
