namespace Lisp.Interface
{
    public interface IEnvironment
    {
        ITopEnvironment TopEnvironment { get; }

        void AddSymbol(string name, ISExpression sExpression);
        bool TryGetSymbol(string name, out ISExpression sExpression);
    }
}
