namespace Lisp.Interface
{
    public interface ITopEnvironment
    {
        void AddSymbol(string name, ISExpression sExpression);
        bool TryGetSymbol(string name, out ISExpression sExpression);
    }
}
