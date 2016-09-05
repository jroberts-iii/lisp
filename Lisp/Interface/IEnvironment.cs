namespace Lisp.Interface
{
    public interface IEnvironment
    {
        IEnvironment EnclosingEnvironment { get; }
        IEnvironment GlobalEnvironment { get; }
        ISExpression[] Parameters { get; }
        void AddSymbol(string name, ISExpression sExpression);
        IEnvironment Push(IEnvironment closureEnvironment, ISExpression[] parameters);
        bool TryGetSymbol(string name, out ISExpression sExpression);
    }
}
